using Sportiva.Contracts.TimeSlots;
using Sportiva.Extensions;
using Sportiva.Services;

namespace Sportiva.Controllers;

[Route("courts/{courtId}/timeslots")]
[ApiController]
[Authorize]
public class TimeSlotController(ITimeSlotService timeSlotService) : ControllerBase
{
    private readonly ITimeSlotService _timeSlotService = timeSlotService;
    private const int ClientClosedRequestStatusCode = 499;

    // POST /courts/{courtId}/timeslots
    [HttpPost]
    public async Task<IActionResult> CreateTimeSlot(string courtId, [FromBody] CreateTimeSlotRequest request, CancellationToken ct)
    {
        try
        {
            var result = await _timeSlotService.CreateTimeSlotAsync(User.GetUserId()!, courtId, request, ct);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(ClientClosedRequestStatusCode, new { message = "Request was cancelled by the client." });
        }
    }

    // POST /courts/{courtId}/timeslots/bulk
    [HttpPost("bulk")]
    public async Task<IActionResult> BulkCreateTimeSlots(string courtId, [FromBody] IReadOnlyList<CreateTimeSlotRequest> requests, CancellationToken ct)
    {
        try
        {
            var result = await _timeSlotService.BulkCreateTimeSlotsAsync(User.GetUserId()!, courtId, requests, ct);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(ClientClosedRequestStatusCode, new { message = "Request was cancelled by the client." });
        }
    }

    // GET /courts/{courtId}/timeslots
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetTimeSlots(string courtId, [FromQuery] DateOnly? date, [FromQuery] bool? available, CancellationToken ct)
    {
        try
        {
            var result = await _timeSlotService.GetTimeSlotsAsync(courtId, date, available, ct);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(ClientClosedRequestStatusCode, new { message = "Request was cancelled by the client." });
        }
    }

    // GET /courts/{courtId}/timeslots/{slotId}
    [HttpGet("{slotId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTimeSlot(string courtId, string slotId, CancellationToken ct)
    {
        try
        {
            var result = await _timeSlotService.GetTimeSlotAsync(courtId, slotId, ct);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(ClientClosedRequestStatusCode, new { message = "Request was cancelled by the client." });
        }
    }

    // PUT /courts/{courtId}/timeslots/{slotId}
    [HttpPut("{slotId}")]
    public async Task<IActionResult> UpdateTimeSlot(string courtId, string slotId, [FromBody] CreateTimeSlotRequest request, CancellationToken ct)
    {
        try
        {
            var result = await _timeSlotService.UpdateTimeSlotAsync(User.GetUserId()!, courtId, slotId, request, ct);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(ClientClosedRequestStatusCode, new { message = "Request was cancelled by the client." });
        }
    }

    // DELETE /courts/{courtId}/timeslots/{slotId}
    [HttpDelete("{slotId}")]
    public async Task<IActionResult> DeleteTimeSlot(string courtId, string slotId, CancellationToken ct)
    {
        try
        {
            var result = await _timeSlotService.DeleteTimeSlotAsync(User.GetUserId()!, courtId, slotId, ct);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(ClientClosedRequestStatusCode, new { message = "Request was cancelled by the client." });
        }
    }
}
