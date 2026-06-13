using Sportiva.Contracts.Common;
using Sportiva.Contracts.Matches;

namespace Sportiva.Services;

public interface IFriendlyMatchService
{
    // ── Match Queries ──────────────────────────────────────────────
    Task<Result<FriendlyMatchResponse>> GetMatchAsync(
        string matchId, string? currentUserId,
        CancellationToken ct = default);

    Task<PaginatedList<FriendlyMatchResponse>> GetMatchesAsync(
        string? currentUserId, RequestFilters filters,
        CancellationToken ct = default);

    Task<PaginatedList<FriendlyMatchResponse>> GetMatchesByOrganizerAsync(
        string organizerId, string? currentUserId, RequestFilters filters,
        CancellationToken ct = default);

    /// <summary>Returns matches the current user has joined.</summary>
    Task<PaginatedList<FriendlyMatchResponse>> GetMyMatchesAsync(
        string userId, RequestFilters filters,
        CancellationToken ct = default);

    // ── Match Commands ─────────────────────────────────────────────
    Task<Result<FriendlyMatchResponse>> CreateMatchAsync(
        string organizerId, CreateFriendlyMatchRequest request,
        CancellationToken ct = default);

    Task<Result> CancelMatchAsync(
        string matchId, string currentUserId,
        CancellationToken ct = default);

    // ── Join Request Queries ───────────────────────────────────────
    /// <summary>Organizer views all join requests for their match.</summary>
    Task<PaginatedList<MatchJoinRequestResponse>> GetJoinRequestsAsync(
        string matchId, string currentUserId, RequestFilters filters,
        CancellationToken ct = default);

    // ── Join Request Commands ──────────────────────────────────────
    /// <summary>Player submits a request to join a match.</summary>
    Task<Result<MatchJoinRequestResponse>> RequestToJoinAsync(
        string userId, JoinMatchRequest request,
        CancellationToken ct = default);

    /// <summary>Organizer accepts or rejects a join request.</summary>
    Task<Result<MatchJoinRequestResponse>> ReviewJoinRequestAsync(
        string requestId, string currentUserId, ReviewJoinRequestRequest request,
        CancellationToken ct = default);

    /// <summary>Player withdraws their own pending join request.</summary>
    Task<Result> WithdrawJoinRequestAsync(
        string requestId, string currentUserId,
        CancellationToken ct = default);
}
