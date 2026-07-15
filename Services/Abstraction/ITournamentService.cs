namespace Sportiva.Services;

public interface ITournamentService
{
    // ── Tournaments ──────────────────────────────────────────────────────────

    //Task<Result<PaginatedList<TournamentResponse>>> GetTournamentsAsync(
    //    string? currentUserId, RequestFilters filters,
    //    SportType? sport = null, TournamentStatus? status = null, string? city = null,
    //    CancellationToken ct = default);

    //Task<Result<TournamentResponse>> GetTournamentAsync(
    //    string tournamentId, string? currentUserId = null, CancellationToken ct = default);

    //Task<Result<TournamentResponse>> CreateTournamentAsync(
    //    string userId, CreateTournamentRequest request, CancellationToken ct = default);

    //Task<Result<TournamentResponse>> UpdateTournamentAsync(
    //    string userId, string tournamentId, CreateTournamentRequest request, CancellationToken ct = default);

    //Task<Result> CancelTournamentAsync(
    //    string userId, string tournamentId, CancellationToken ct = default);

    //Task<Result> JoinTournamentAsync(
    //    string userId, string tournamentId, CancellationToken ct = default);

    //Task<Result> LeaveTournamentAsync(
    //    string userId, string tournamentId, CancellationToken ct = default);

    //Task<Result<PaginatedList<ParticipantSummary>>> GetTournamentParticipantsAsync(
    //    string tournamentId, RequestFilters filters, CancellationToken ct = default);

    //Task<Result<PaginatedList<TournamentResponse>>> GetMyTournamentsAsync(
    //    string userId, RequestFilters filters, CancellationToken ct = default);

    //// ── Tournament Matches (bracket) ─────────────────────────────────────────

    //Task<Result<IReadOnlyList<TournamentMatchResponse>>> GetTournamentMatchesAsync(
    //    string tournamentId, int? round = null, CancellationToken ct = default);

    //Task<Result<TournamentMatchResponse>> GetTournamentMatchAsync(
    //    string tournamentId, string matchId, CancellationToken ct = default);

    //Task<Result<TournamentMatchResponse>> ScheduleTournamentMatchAsync(
    //    string userId, string tournamentId, CreateTournamentMatchRequest request, CancellationToken ct = default);

    //Task<Result<TournamentMatchResponse>> UpdateTournamentMatchAsync(
    //    string userId, string tournamentId, string matchId, CreateTournamentMatchRequest request, CancellationToken ct = default);

    //Task<Result> CancelTournamentMatchAsync(
    //    string userId, string tournamentId, string matchId, CancellationToken ct = default);

    //Task<Result<TournamentMatchResponse>> SetMatchWinnerAsync(
    //    string userId, string tournamentId, string matchId, SetTournamentMatchWinnerRequest request, CancellationToken ct = default);
}
