using Sportiva.Contracts.Notifications;
using Sportiva.Extensions;
using Sportiva.Services;

namespace Sportiva.Controllers;

[Route("notifications")]
[ApiController]
[Authorize]
public class NotificationsController(INotificationService notificationService) : ControllerBase
{
    private readonly INotificationService _notificationService = notificationService;

    [HttpGet]
    public async Task<IActionResult> GetNotifications(
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        var result = await _notificationService.GetNotificationsAsync(User.GetUserId()!, pageNumber, pageSize, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("counters")]
    public async Task<IActionResult> GetNotificationCounters(CancellationToken ct)
    {
        var result = await _notificationService.GetNotificationCountersAsync(User.GetUserId()!, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{notificationId}/read")]
    public async Task<IActionResult> MarkAsRead(string notificationId, CancellationToken ct)
    {
        var result = await _notificationService.MarkAsReadAsync(User.GetUserId()!, notificationId, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken ct)
    {
        var result = await _notificationService.MarkAllAsReadAsync(User.GetUserId()!, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpGet("preferences")]
    public async Task<IActionResult> GetPreferences(CancellationToken ct)
    {
        var result = await _notificationService.GetPreferencesAsync(User.GetUserId()!, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("preferences")]
    public async Task<IActionResult> UpdatePreferences(
        [FromBody] BulkUpdateNotificationPreferencesRequest request, CancellationToken ct)
    {
        var result = await _notificationService.UpdatePreferencesAsync(User.GetUserId()!, request, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
