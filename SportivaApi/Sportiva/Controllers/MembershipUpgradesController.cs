// Controllers/MembershipUpgradesController.cs
using Sportiva.Contracts.Common;
using Sportiva.Contracts.Memberships;
using Sportiva.Contracts.Shared.Enums;
using Sportiva.Extensions;
using Sportiva.Services;

namespace Sportiva.Controllers;

[Route("membership-requests")]
[ApiController]
[Authorize]
public class MembershipUpgradesController(IMembershipUpgradeService membershipUpgradeService) : ControllerBase
{
    private readonly IMembershipUpgradeService _membershipUpgradeService = membershipUpgradeService;

    // ════════════════════════════════════════════════════════════════
    //  للمستخدم العادي
    // ════════════════════════════════════════════════════════════════

    //for all authenticated users
    // GET /me/membership-request
    [HttpGet("/me/membership-request")]
    public async Task<IActionResult> GetMyUpgradeRequest(CancellationToken ct)
    {
        var result = await _membershipUpgradeService.GetMyUpgradeRequestAsync(User.GetUserId()!, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    //for all authenticated users
    // POST /membership-requests
    [HttpPost]
    public async Task<IActionResult> SubmitUpgradeRequest(
        [FromBody] CreateMembershipUpgradeRequest request, CancellationToken ct)
    {
        var result = await _membershipUpgradeService.SubmitUpgradeRequestAsync(User.GetUserId()!, request, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    // ════════════════════════════════════════════════════════════════
    //  للأدمن فقط
    // ════════════════════════════════════════════════════════════════

    //for Admin
    // GET /membership-requests
    [HttpGet]
    [Authorize(Roles = DefaultRoles.Admin.Name)]
    public async Task<IActionResult> GetUpgradeRequests(
        [FromQuery] RequestFilters filters, [FromQuery] RequestStatus? status, CancellationToken ct)
    {
        var result = await _membershipUpgradeService.GetUpgradeRequestsAsync(filters, status, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    //for Admin
    // GET /membership-requests/{requestId}
    [HttpGet("{requestId}")]
    [Authorize(Roles = DefaultRoles.Admin.Name)]
    public async Task<IActionResult> GetUpgradeRequest(string requestId, CancellationToken ct)
    {
        var result = await _membershipUpgradeService.GetUpgradeRequestAsync(requestId, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    //for Admin
    // POST /membership-requests/{requestId}/approve
    [HttpPost("{requestId}/approve")]
    [Authorize(Roles = DefaultRoles.Admin.Name)]
    public async Task<IActionResult> ApproveUpgradeRequest(string requestId, CancellationToken ct)
    {
        var request = new ReviewMembershipUpgradeRequest(RequestStatusDto.Approved);
        var result = await _membershipUpgradeService.ReviewUpgradeRequestAsync(User.GetUserId()!, requestId, request, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    //for Admin
    // POST /membership-requests/{requestId}/reject
    [HttpPost("{requestId}/reject")]
    [Authorize(Roles = DefaultRoles.Admin.Name)]
    public async Task<IActionResult> RejectUpgradeRequest(string requestId, CancellationToken ct)
    {
        var request = new ReviewMembershipUpgradeRequest(RequestStatusDto.Rejected);
        var result = await _membershipUpgradeService.ReviewUpgradeRequestAsync(User.GetUserId()!, requestId, request, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}