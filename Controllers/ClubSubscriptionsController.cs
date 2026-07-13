using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sportiva.Contracts.Common;
using Sportiva.Contracts.Subscriptions;
using Sportiva.Extensions;
using Sportiva.Services;

namespace Sportiva.Controllers;

[Route("clubs/{clubId}")]
[ApiController]
[Authorize]
public class ClubSubscriptionsController(IClubSubscriptionService clubSubscriptionService) : ControllerBase
{
    private readonly IClubSubscriptionService _clubSubscriptionService = clubSubscriptionService;

    // GET /clubs/{clubId}/subscription
    [HttpGet("subscription")]
    public async Task<IActionResult> GetActiveSubscription(string clubId, CancellationToken ct)
    {
        var result = await _clubSubscriptionService.GetActiveSubscriptionAsync(User.GetUserId()!, clubId, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    // GET /clubs/{clubId}/subscriptions
    [HttpGet("subscriptions")]
    public async Task<IActionResult> GetSubscriptionHistory(string clubId, [FromQuery] RequestFilters filters, CancellationToken ct)
    {
        var result = await _clubSubscriptionService.GetSubscriptionHistoryAsync(User.GetUserId()!, clubId, filters, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    // POST /clubs/{clubId}/subscriptions
    [HttpPost("subscriptions")]
    public async Task<IActionResult> Subscribe(string clubId, [FromBody] CreateClubSubscriptionRequest request, CancellationToken ct)
    {
        var result = await _clubSubscriptionService.SubscribeAsync(User.GetUserId()!, clubId, request, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    // POST /clubs/{clubId}/subscriptions/renew
    [HttpPost("subscriptions/renew")]
    public async Task<IActionResult> RenewSubscription(string clubId, CancellationToken ct)
    {
        var result = await _clubSubscriptionService.RenewSubscriptionAsync(User.GetUserId()!, clubId, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    // POST /clubs/{clubId}/subscriptions/cancel
    [HttpPost("subscriptions/cancel")]
    public async Task<IActionResult> CancelSubscription(string clubId, CancellationToken ct)
    {
        var result = await _clubSubscriptionService.CancelSubscriptionAsync(User.GetUserId()!, clubId, ct);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }
}
