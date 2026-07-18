using Sportiva.Contracts.Common;
using Sportiva.Contracts.Tournaments;
using Sportiva.Extensions;
using Sportiva.Services;
using Sportiva.Enums;

namespace Sportiva.Controllers;

[Route("tournaments")]
[ApiController]
[Authorize]
public class TournamentsController(ITournamentService tournamentService) : ControllerBase
{
    private readonly ITournamentService _tournamentService = tournamentService;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetTournaments(
        [FromQuery] RequestFilters filters, [FromQuery] SportType? sport, [FromQuery] TournamentStatus? status, [FromQuery] string? city, CancellationToken ct)
    {
        var result = await _tournamentService.GetTournamentsAsync(User.GetUserId(), filters, sport, status, city, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyTournaments([FromQuery] RequestFilters filters, CancellationToken ct)
    {
        var result = await _tournamentService.GetMyTournamentsAsync(User.GetUserId()!, filters, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("{tournamentId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTournament(string tournamentId, CancellationToken ct)
    {
        var result = await _tournamentService.GetTournamentAsync(tournamentId, User.GetUserId(), ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost]
    public async Task<IActionResult> CreateTournament([FromBody] CreateTournamentRequest request, CancellationToken ct)
    {
        var result = await _tournamentService.CreateTournamentAsync(User.GetUserId()!, request, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{tournamentId}")]
    public async Task<IActionResult> UpdateTournament(string tournamentId, [FromBody] CreateTournamentRequest request, CancellationToken ct)
    {
        var result = await _tournamentService.UpdateTournamentAsync(User.GetUserId()!, tournamentId, request, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpDelete("{tournamentId}")]
    public async Task<IActionResult> CancelTournament(string tournamentId, CancellationToken ct)
    {
        var result = await _tournamentService.CancelTournamentAsync(User.GetUserId()!, tournamentId, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPost("{tournamentId}/join")]
    public async Task<IActionResult> JoinTournament(string tournamentId, CancellationToken ct)
    {
        var result = await _tournamentService.JoinTournamentAsync(User.GetUserId()!, tournamentId, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPost("{tournamentId}/leave")]
    public async Task<IActionResult> LeaveTournament(string tournamentId, CancellationToken ct)
    {
        var result = await _tournamentService.LeaveTournamentAsync(User.GetUserId()!, tournamentId, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpGet("{tournamentId}/participants")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTournamentParticipants(string tournamentId, [FromQuery] RequestFilters filters, CancellationToken ct)
    {
        var result = await _tournamentService.GetTournamentParticipantsAsync(tournamentId, filters, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    // ── Tournament Matches (bracket) ──

    [HttpGet("{tournamentId}/matches")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTournamentMatches(string tournamentId, [FromQuery] int? round, CancellationToken ct)
    {
        var result = await _tournamentService.GetTournamentMatchesAsync(tournamentId, round, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("{tournamentId}/matches/{matchId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTournamentMatch(string tournamentId, string matchId, CancellationToken ct)
    {
        var result = await _tournamentService.GetTournamentMatchAsync(tournamentId, matchId, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("{tournamentId}/matches")]
    public async Task<IActionResult> ScheduleTournamentMatch(string tournamentId, [FromBody] CreateTournamentMatchRequest request, CancellationToken ct)
    {
        var result = await _tournamentService.ScheduleTournamentMatchAsync(User.GetUserId()!, tournamentId, request, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{tournamentId}/matches/{matchId}")]
    public async Task<IActionResult> UpdateTournamentMatch(string tournamentId, string matchId, [FromBody] CreateTournamentMatchRequest request, CancellationToken ct)
    {
        var result = await _tournamentService.UpdateTournamentMatchAsync(User.GetUserId()!, tournamentId, matchId, request, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpDelete("{tournamentId}/matches/{matchId}")]
    public async Task<IActionResult> CancelTournamentMatch(string tournamentId, string matchId, CancellationToken ct)
    {
        var result = await _tournamentService.CancelTournamentMatchAsync(User.GetUserId()!, tournamentId, matchId, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPut("{tournamentId}/matches/{matchId}/winner")]
    public async Task<IActionResult> SetMatchWinner(string tournamentId, string matchId, [FromBody] SetTournamentMatchWinnerRequest request, CancellationToken ct)
    {
        var result = await _tournamentService.SetMatchWinnerAsync(User.GetUserId()!, tournamentId, matchId, request, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}
