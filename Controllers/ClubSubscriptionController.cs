using Sportiva.Contracts.Common;
using Sportiva.Contracts.Subscriptions;
using Sportiva.Extensions;
using Sportiva.Services;

namespace Sportiva.Controllers;

[Route("clubs/{clubId}/subscriptions")]
[ApiController]
[Authorize]
public class ClubSubscriptionController(IClubSubscriptionService clubSubscriptionService) : ControllerBase
{
    private readonly IClubSubscriptionService _clubSubscriptionService = clubSubscriptionService;
    private const int ClientClosedRequestStatusCode = 499;

    // POST /clubs/{clubId}/subscriptions
    [HttpPost]
    public async Task<IActionResult> Subscribe([FromRoute] string clubId, [FromBody] CreateClubSubscriptionRequest request, CancellationToken ct)
    {
        try
        {
            var result = await _clubSubscriptionService.SubscribeAsync(User.GetUserId()!, clubId, request, ct);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(ClientClosedRequestStatusCode, new { message = "Request was cancelled by the client." });
        }
    }

    // DELETE /clubs/{clubId}/subscriptions
    [HttpDelete]
    public async Task<IActionResult> CancelSubscription([FromRoute] string clubId, CancellationToken ct)
    {
        try
        {
            var result = await _clubSubscriptionService.CancelSubscriptionAsync(User.GetUserId()!, clubId, ct);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(ClientClosedRequestStatusCode, new { message = "Request was cancelled by the client." });
        }
    }

    // GET /clubs/{clubId}/subscriptions/active
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveSubscription([FromRoute] string clubId, CancellationToken ct)
    {
        try
        {
            var result = await _clubSubscriptionService.GetActiveSubscriptionAsync(User.GetUserId()!, clubId, ct);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(ClientClosedRequestStatusCode, new { message = "Request was cancelled by the client." });
        }
    }

    // GET /clubs/{clubId}/subscriptions/history
    [HttpGet("history")]
    public async Task<IActionResult> GetSubscriptionHistory([FromRoute] string clubId, [FromQuery] RequestFilters filters, CancellationToken ct)
    {
        try
        {
            var result = await _clubSubscriptionService.GetSubscriptionHistoryAsync(User.GetUserId()!, clubId, filters, ct);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(ClientClosedRequestStatusCode, new { message = "Request was cancelled by the client." });
        }
    }

    // POST /clubs/{clubId}/subscriptions/renew
    [HttpPost("renew")]
    public async Task<IActionResult> RenewSubscription([FromRoute] string clubId, CancellationToken ct)
    {
        try
        {
            var result = await _clubSubscriptionService.RenewSubscriptionAsync(User.GetUserId()!, clubId, ct);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(ClientClosedRequestStatusCode, new { message = "Request was cancelled by the client." });
        }
    }
}
