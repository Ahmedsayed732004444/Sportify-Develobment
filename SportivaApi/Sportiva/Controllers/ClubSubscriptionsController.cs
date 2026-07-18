using Sportiva.Contracts.Common;
using Sportiva.Contracts.Subscriptions;
using Sportiva.Extensions;
using Sportiva.Services;

namespace Sportiva.Controllers;

[Route("clubs/{clubId}/subscriptions")]
[ApiController]
[Authorize]
public class ClubSubscriptionsController(IClubSubscriptionService clubSubscriptionService) : ControllerBase
{
    private readonly IClubSubscriptionService _clubSubscriptionService = clubSubscriptionService;

    //for club owners
    // GET /clubs/{clubId}/subscriptions/active
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveSubscription(string clubId, CancellationToken ct)
    {
        var result = await _clubSubscriptionService.GetActiveSubscriptionAsync(User.GetUserId()!, clubId, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    //for club owners
    // GET /clubs/{clubId}/subscriptions
    [HttpGet]
    public async Task<IActionResult> GetSubscriptionHistory(string clubId, [FromQuery] RequestFilters filters, CancellationToken ct)
    {
        var result = await _clubSubscriptionService.GetSubscriptionHistoryAsync(User.GetUserId()!, clubId, filters, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    //for club owners
    // POST /clubs/{clubId}/subscriptions
    [HttpPost]
    public async Task<IActionResult> Subscribe(string clubId, [FromBody] CreateClubSubscriptionRequest request, CancellationToken ct)
    {
        var result = await _clubSubscriptionService.SubscribeAsync(User.GetUserId()!, clubId, request, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    //for club owners
    // POST /clubs/{clubId}/subscriptions/renew
    [HttpPost("renew")]
    public async Task<IActionResult> RenewSubscription(string clubId, CancellationToken ct)
    {
        var result = await _clubSubscriptionService.RenewSubscriptionAsync(User.GetUserId()!, clubId, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    //for club owners
    // DELETE /clubs/{clubId}/subscriptions/active
    [HttpDelete("active")]
    public async Task<IActionResult> CancelSubscription(string clubId, CancellationToken ct)
    {
        var result = await _clubSubscriptionService.CancelSubscriptionAsync(User.GetUserId()!, clubId, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
