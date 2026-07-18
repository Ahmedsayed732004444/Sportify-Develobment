using Sportiva.Contracts.Common;
using Sportiva.Contracts.Courts;
using Sportiva.Extensions;
using Sportiva.Services;

namespace Sportiva.Controllers;

[Route("clubs/{clubId}/courts")]
[ApiController]
[Authorize]
public class CourtsController(ICourtService courtService) : ControllerBase
{
    private readonly ICourtService _courtService = courtService;

    // ════════════════════════════════════════════════════════════════
    //  Public — مش تابعة لنادي معين، عشان كده absolute route (/courts)
    // ════════════════════════════════════════════════════════════════

    //for all users
    // GET /courts
    [HttpGet("/courts")]
    [AllowAnonymous]
    public async Task<IActionResult> SearchCourts(
        [FromQuery] RequestFilters filters,
        [FromQuery] SportType? sport,
        [FromQuery] string? city,
        [FromQuery] DateOnly? date,
        CancellationToken ct)
    {
        var result = await _courtService.SearchCourtsAsync(User.GetUserId(), filters, sport, city, date, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    //for all users
    // GET /courts/{courtId}/availability
    [HttpGet("/courts/{courtId}/availability")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCourtAvailability(string courtId, [FromQuery] DateOnly date, CancellationToken ct)
    {
        var result = await _courtService.GetCourtAvailabilityAsync(courtId, date, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    // ════════════════════════════════════════════════════════════════
    //  Club-scoped
    // ════════════════════════════════════════════════════════════════

    //for all users
    // GET /clubs/{clubId}/courts
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetClubCourts(string clubId, [FromQuery] RequestFilters filters, CancellationToken ct)
    {
        var result = await _courtService.GetClubCourtsAsync(clubId, User.GetUserId(), filters, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    //for all users
    // GET /clubs/{clubId}/courts/{courtId}
    [HttpGet("{courtId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCourt(string clubId, string courtId, CancellationToken ct)
    {
        var result = await _courtService.GetCourtAsync(clubId, courtId, User.GetUserId(), ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    //for club owners
    // POST /clubs/{clubId}/courts
    [HttpPost]
    public async Task<IActionResult> CreateCourt(string clubId, [FromForm] CreateCourtRequest request, CancellationToken ct)
    {
        var result = await _courtService.CreateCourtAsync(User.GetUserId()!, clubId, request, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    //for club owners
    // PUT /clubs/{clubId}/courts/{courtId}
    [HttpPut("{courtId}")]
    public async Task<IActionResult> UpdateCourt(string clubId, string courtId, [FromForm] UpdateCourtRequest request, CancellationToken ct)
    {
        var result = await _courtService.UpdateCourtAsync(User.GetUserId()!, clubId, courtId, request, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    //for club owners
    // DELETE /clubs/{clubId}/courts/{courtId}
    [HttpDelete("{courtId}")]
    public async Task<IActionResult> DeleteCourt(string clubId, string courtId, CancellationToken ct)
    {
        var result = await _courtService.DeleteCourtAsync(User.GetUserId()!, clubId, courtId, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    //for club owners
    // PATCH /clubs/{clubId}/courts/{courtId}/status
    [HttpPatch("{courtId}/status")]
    public async Task<IActionResult> ToggleCourtStatus(string clubId, string courtId, CancellationToken ct)
    {
        var result = await _courtService.ToggleCourtStatusAsync(User.GetUserId()!, clubId, courtId, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
