using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sportiva.Entities;
using Sportiva.Enums;
using Sportiva.Extensions;
using Sportiva.Persistence;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sportiva.Controllers;

[ApiController]
[Route("clubs/{clubId}/subscriptions/requests")]
[Authorize]
public class SubscriptionRequestsController(ApplicationDbContext context) : ControllerBase
{
    private readonly ApplicationDbContext _context = context;

    [HttpPost]
    public async Task<IActionResult> CreateRequest(string clubId, [FromBody] CreateSubscriptionRequestDto request, CancellationToken ct)
    {
        var userId = User.GetUserId();
        var club = await _context.Clubs.FirstOrDefaultAsync(c => c.Id == clubId && !c.IsDeleted, ct);
        if (club == null)
            return NotFound("Club not found.");

        if (club.OwnerId != userId)
            return Forbid();

        var subRequest = new SubscriptionRequest
        {
            ClubId = clubId,
            PlanId = request.PlanId,
            RequestType = (SubscriptionRequestType)request.RequestType,
            Phone = request.Phone,
            Note = request.Note,
            Status = RequestStatus.Pending,
            RequestedAt = DateTime.UtcNow
        };

        await _context.SubscriptionRequests.AddAsync(subRequest, ct);
        await _context.SaveChangesAsync(ct);

        return Ok(subRequest);
    }

    [HttpGet]
    public async Task<IActionResult> GetRequests(string clubId, CancellationToken ct)
    {
        var userId = User.GetUserId();
        var club = await _context.Clubs.FirstOrDefaultAsync(c => c.Id == clubId && !c.IsDeleted, ct);
        if (club == null)
            return NotFound("Club not found.");

        if (club.OwnerId != userId)
            return Forbid();

        var list = await _context.SubscriptionRequests
            .Include(r => r.Plan)
            .Where(r => r.ClubId == clubId)
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync(ct);

        return Ok(list);
    }
}

[ApiController]
[Authorize(Roles = "Admin")]
public class AdminSubscriptionRequestsController(ApplicationDbContext context) : ControllerBase
{
    private readonly ApplicationDbContext _context = context;

    [HttpGet("admin/subscription-requests")]
    public async Task<IActionResult> GetAllRequests(CancellationToken ct)
    {
        var list = await _context.SubscriptionRequests
            .Include(r => r.Plan)
            .Include(r => r.Club)
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync(ct);

        return Ok(list);
    }

    [HttpPost("admin/subscription-requests/{requestId}/approve")]
    public async Task<IActionResult> ApproveRequest(string requestId, CancellationToken ct)
    {
        var subReq = await _context.SubscriptionRequests
            .Include(r => r.Plan)
            .FirstOrDefaultAsync(r => r.Id == requestId, ct);

        if (subReq == null)
            return NotFound("Subscription request not found.");

        subReq.Status = RequestStatus.Approved;
        subReq.ReviewedAt = DateTime.UtcNow;

        // Process actual subscription upgrade/renewal
        var activeSub = await _context.ClubSubscriptions
            .FirstOrDefaultAsync(s => s.ClubId == subReq.ClubId && s.StartDate <= DateTime.UtcNow && s.EndDate >= DateTime.UtcNow && !s.IsDeleted, ct);

        if (subReq.RequestType == SubscriptionRequestType.Renew && activeSub != null)
        {
            // Extend existing subscription by 1 year
            activeSub.EndDate = activeSub.EndDate.AddYears(1);
        }
        else
        {
            // Upgrade or brand-new subscription
            if (activeSub != null)
            {
                // Soft delete or deactivate old active subscription
                activeSub.IsDeleted = true;
            }

            var newSub = new ClubSubscription
            {
                ClubId = subReq.ClubId,
                PlanId = subReq.PlanId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddYears(1)
            };
            await _context.ClubSubscriptions.AddAsync(newSub, ct);
        }

        await _context.SaveChangesAsync(ct);
        return Ok("Request approved successfully.");
    }

    [HttpPost("admin/subscription-requests/{requestId}/reject")]
    public async Task<IActionResult> RejectRequest(string requestId, CancellationToken ct)
    {
        var subReq = await _context.SubscriptionRequests
            .FirstOrDefaultAsync(r => r.Id == requestId, ct);

        if (subReq == null)
            return NotFound("Subscription request not found.");

        subReq.Status = RequestStatus.Rejected;
        subReq.ReviewedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return Ok("Request rejected.");
    }
}

public record CreateSubscriptionRequestDto(
    string PlanId,
    int RequestType,
    string Phone,
    string? Note
);
