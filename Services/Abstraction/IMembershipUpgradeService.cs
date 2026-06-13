using Sportiva.Contracts.Common;
using Sportiva.Contracts.Memberships;

namespace Sportiva.Services;

public interface IMembershipUpgradeService
{
    // ── Queries ────────────────────────────────────────────────────
    Task<Result<MembershipUpgradeResponse>> GetRequestAsync(
        string requestId, string currentUserId,
        CancellationToken ct = default);

    /// <summary>User's own request history.</summary>
    Task<PaginatedList<MembershipUpgradeResponse>> GetMyRequestsAsync(
        string userId, RequestFilters filters,
        CancellationToken ct = default);

    /// <summary>Admin view — all requests with optional status filter.</summary>
    Task<PaginatedList<MembershipUpgradeResponse>> GetAllRequestsAsync(
        RequestFilters filters, CancellationToken ct = default);

    // ── Commands ───────────────────────────────────────────────────
    /// <summary>
    /// User submits a new upgrade request.
    /// Fails if a Pending request already exists (PendingUpgradeRequestExists).
    /// </summary>
    Task<Result<MembershipUpgradeResponse>> CreateRequestAsync(
        string userId, CreateMembershipUpgradeRequest request,
        CancellationToken ct = default);

    /// <summary>Admin approves or rejects the request.</summary>
    Task<Result<MembershipUpgradeResponse>> ReviewRequestAsync(
        string requestId, string adminId, ReviewMembershipUpgradeRequest request,
        CancellationToken ct = default);

    /// <summary>User cancels their own pending request.</summary>
    Task<Result> CancelRequestAsync(
        string requestId, string currentUserId,
        CancellationToken ct = default);
}
