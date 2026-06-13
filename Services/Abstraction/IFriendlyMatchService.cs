using Sportiva.Contracts.Common;
using Sportiva.Contracts.Matches;
using Sportiva.Contracts.Shared.Summaries;
using Sportiva.Enums;

namespace Sportiva.Services;

public interface IFriendlyMatchService
{
    Task<Result<PaginatedList<FriendlyMatchResponse>>> GetMatchesAsync(
        string? currentUserId, RequestFilters filters,
        SportType? sport = null, DateOnly? date = null, string? city = null,
        CancellationToken ct = default);

    Task<Result<FriendlyMatchResponse>> GetMatchAsync(
        string matchId, string? currentUserId = null, CancellationToken ct = default);

    Task<Result<FriendlyMatchResponse>> CreateMatchAsync(
        string userId, CreateFriendlyMatchRequest request, CancellationToken ct = default);

    Task<Result<FriendlyMatchResponse>> UpdateMatchAsync(
        string userId, string matchId, CreateFriendlyMatchRequest request, CancellationToken ct = default);

    Task<Result> CancelMatchAsync(
        string userId, string matchId, CancellationToken ct = default);

    Task<Result<IReadOnlyList<ParticipantSummary>>> GetParticipantsAsync(
        string matchId, CancellationToken ct = default);

    Task<Result> LeaveMatchAsync(
        string userId, string matchId, CancellationToken ct = default);

    Task<Result<PaginatedList<FriendlyMatchResponse>>> GetMyMatchesAsync(
        string userId, RequestFilters filters, string? role = null, CancellationToken ct = default);

    Task<Result<PaginatedList<FriendlyMatchResponse>>> GetCourtMatchesAsync(
        string courtId, RequestFilters filters, CancellationToken ct = default);
}
