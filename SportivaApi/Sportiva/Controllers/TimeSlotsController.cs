using Sportiva.Contracts.TimeSlots;
using Sportiva.Extensions;
using Sportiva.Services;

namespace Sportiva.Controllers;

[Route("courts/{courtId}/time-slots")]
[ApiController]
[Authorize]
public class TimeSlotsController(ITimeSlotService timeSlotService) : ControllerBase
{
    private readonly ITimeSlotService _timeSlotService = timeSlotService;

    // ════════════════════════════════════════════════════════════════
    //  Public — أي حد يقدر يشوف الـ time slots بتاعة ملعب معين
    // ════════════════════════════════════════════════════════════════

    //for all users
    // GET /courts/{courtId}/time-slots
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetTimeSlots(
        string courtId, [FromQuery] DateOnly? date, [FromQuery] bool? available, CancellationToken ct)
    {
        var result = await _timeSlotService.GetTimeSlotsAsync(courtId, date, available, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    //for all users
    // GET /courts/{courtId}/time-slots/{slotId}
    [HttpGet("{slotId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTimeSlot(string courtId, string slotId, CancellationToken ct)
    {
        var result = await _timeSlotService.GetTimeSlotAsync(courtId, slotId, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    // ════════════════════════════════════════════════════════════════
    //  Owner control
    // ════════════════════════════════════════════════════════════════

    //for club owners
    // PATCH /courts/{courtId}/time-slots/availability
    [HttpPatch("availability")]
    public async Task<IActionResult> SetTimeSlotsAvailability(
        string courtId, [FromBody] SetTimeSlotsAvailabilityRequest request, CancellationToken ct)
    {
        var result = await _timeSlotService.SetTimeSlotsAvailabilityAsync(
            User.GetUserId()!, courtId, request.SlotIds, request.IsActive, ct);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    // for club owners (تشغيل يدوي بره الـ Hangfire job، مفيد لو الملعب اتعمل ومحتاج
    // توليد فوري أو لو عايز تمدد الأسبوع تاني يدوي)
    // POST /courts/{courtId}/time-slots/generate
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateWeeklyTimeSlots(string courtId, CancellationToken ct)
    {
        var result = await _timeSlotService.GenerateWeeklyTimeSlotsForCourtAsync(courtId, ct);
        return result.IsSuccess ? Ok(new { GeneratedCount = result.Value }) : result.ToProblem();
    }
}