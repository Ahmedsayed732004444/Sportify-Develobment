using Sportiva.Contracts.Common;
using Sportiva.Contracts.Shared.Summaries;
using Sportiva.Contracts.Tournaments;

namespace Sportiva.Services;

public interface ITournamentService
{
    // ── Tournament Queries ─────────────────────────────────────────
    Task<Result<TournamentResponse>> GetTournamentAsync(
        string tournamentId, string? currentUserId,
        CancellationToken ct = default);

    Task<PaginatedList<TournamentResponse>> GetTournamentsAsync(
        string? currentUserId, RequestFilters filters,
        CancellationToken ct = default);

    Task<PaginatedList<TournamentResponse>> GetTournamentsByOrganizerAsync(
        string organizerId, string? currentUserId, RequestFilters filters,
        CancellationToken ct = default);

    // ── Tournament Commands ────────────────────────────────────────
    Task<Result<TournamentResponse>> CreateTournamentAsync(
        string organizerId, CreateTournamentRequest request,
        CancellationToken ct = default);

    Task<Result> DeleteTournamentAsync(
        string tournamentId, string currentUserId,
        CancellationToken ct = default);

    // ── Participant Management ─────────────────────────────────────
    Task<PaginatedList<ParticipantSummary>> GetParticipantsAsync(
        string tournamentId, RequestFilters filters,
        CancellationToken ct = default);

    Task<Result> JoinTournamentAsync(
        string tournamentId, string userId,
        CancellationToken ct = default);

    Task<Result> LeaveTournamentAsync(
        string tournamentId, string userId,
        CancellationToken ct = default);

    // ── Match Management ───────────────────────────────────────────
    Task<PaginatedList<TournamentMatchResponse>> GetTournamentMatchesAsync(
        string tournamentId, RequestFilters filters,
        CancellationToken ct = default);

    Task<Result<TournamentMatchResponse>> CreateTournamentMatchAsync(
        string currentUserId, CreateTournamentMatchRequest request,
        CancellationToken ct = default);

    Task<Result<TournamentMatchResponse>> SetMatchWinnerAsync(
        string matchId, string currentUserId, SetTournamentMatchWinnerRequest request,
        CancellationToken ct = default);
}
