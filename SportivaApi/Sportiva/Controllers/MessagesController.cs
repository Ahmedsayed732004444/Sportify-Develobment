using Sportiva.Contracts.Common;
using Sportiva.Contracts.Messaging;
using Sportiva.Extensions;
using Sportiva.Services;

namespace Sportiva.Controllers;

[Route("messages")]
[ApiController]
[Authorize]
public class MessagesController(IMessagingService messagingService) : ControllerBase
{
    private readonly IMessagingService _messagingService = messagingService;

    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations([FromQuery] RequestFilters filters, CancellationToken ct)
    {
        var result = await _messagingService.GetConversationsAsync(User.GetUserId()!, filters, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("{otherUserId}")]
    public async Task<IActionResult> GetMessages(string otherUserId, [FromQuery] RequestFilters filters, CancellationToken ct)
    {
        var result = await _messagingService.GetMessagesAsync(User.GetUserId()!, otherUserId, filters, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request, CancellationToken ct)
    {
        var result = await _messagingService.SendMessageAsync(User.GetUserId()!, request, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{otherUserId}/read")]
    public async Task<IActionResult> MarkAsRead(string otherUserId, CancellationToken ct)
    {
        var result = await _messagingService.MarkConversationAsReadAsync(User.GetUserId()!, otherUserId, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpDelete("{messageId}")]
    public async Task<IActionResult> DeleteMessage(string messageId, CancellationToken ct)
    {
        var result = await _messagingService.DeleteMessageAsync(User.GetUserId()!, messageId, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
