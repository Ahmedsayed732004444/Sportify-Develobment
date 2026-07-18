using Sportiva.Contracts.Common;
using Sportiva.Contracts.Matches;
using Sportiva.Extensions;
using Sportiva.Services;

namespace Sportiva.Controllers;

[Route("friendly-matches")]
[ApiController]
[Authorize]
public class FriendlyMatchesController(IFriendlyMatchService matchService) : ControllerBase
{
    private readonly IFriendlyMatchService _matchService = matchService;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetMatches(
        [FromQuery] RequestFilters filters, [FromQuery] SportType? sport, [FromQuery] DateOnly? date, [FromQuery] string? city, CancellationToken ct)
    {
        var result = await _matchService.GetMatchesAsync(User.GetUserId(), filters, sport, date, city, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyMatches(
        [FromQuery] RequestFilters filters, [FromQuery] string? role, CancellationToken ct)
    {
        var result = await _matchService.GetMyMatchesAsync(User.GetUserId()!, filters, role, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("court/{courtId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCourtMatches(string courtId, [FromQuery] RequestFilters filters, CancellationToken ct)
    {
        var result = await _matchService.GetCourtMatchesAsync(courtId, filters, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("{matchId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetMatch(string matchId, CancellationToken ct)
    {
        var result = await _matchService.GetMatchAsync(matchId, User.GetUserId(), ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost]
    public async Task<IActionResult> CreateMatch([FromBody] CreateFriendlyMatchRequest request, CancellationToken ct)
    {
        var result = await _matchService.CreateMatchAsync(User.GetUserId()!, request, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{matchId}")]
    public async Task<IActionResult> UpdateMatch(string matchId, [FromBody] CreateFriendlyMatchRequest request, CancellationToken ct)
    {
        var result = await _matchService.UpdateMatchAsync(User.GetUserId()!, matchId, request, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpDelete("{matchId}")]
    public async Task<IActionResult> CancelMatch(string matchId, CancellationToken ct)
    {
        var result = await _matchService.CancelMatchAsync(User.GetUserId()!, matchId, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpGet("{matchId}/participants")]
    [AllowAnonymous]
    public async Task<IActionResult> GetParticipants(string matchId, CancellationToken ct)
    {
        var result = await _matchService.GetParticipantsAsync(matchId, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("{matchId}/leave")]
    public async Task<IActionResult> LeaveMatch(string matchId, CancellationToken ct)
    {
        var result = await _matchService.LeaveMatchAsync(User.GetUserId()!, matchId, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
