using Sportiva.Contracts.Common;
using Sportiva.Contracts.Shared.Summaries;
using Sportiva.Contracts.Users;

namespace Sportiva.Services;

public interface IProfileService
{
    // ── Profile ──────────────────────────────────────────────────────
    Task<Result<UserProfileResponse>> GetProfileAsync(
        string profileUserId, string? currentUserId = null, CancellationToken ct = default);

    Task<Result<UserProfileResponse>> UpdateProfileInfoAsync(
     string userId, UpdateProfileInfoRequest request, CancellationToken ct = default);

    Task<Result<UserProfileResponse>> UpdateProfilePhotoAsync(
        string userId, UpdateProfilePhotoRequest request, CancellationToken ct = default);

    Task<Result<UserProfileResponse>> UpdateProfileCoverAsync(
        string userId, UpdateProfileCoverRequest request, CancellationToken ct = default);

    // ── Follow / Unfollow ────────────────────────────────────────────
    Task<Result<ToggleFollowResponse>> ToggleFollowAsync(
        string currentUserId, string targetUserId, CancellationToken ct = default);

    // ── Followers / Following ────────────────────────────────────────
    Task<PaginatedList<UserCardSummary>> GetFollowersAsync(
        string profileUserId, string? currentUserId, RequestFilters filters, CancellationToken ct = default);

    Task<PaginatedList<UserCardSummary>> GetFollowingAsync(
        string profileUserId, string? currentUserId, RequestFilters filters, CancellationToken ct = default);

    // ── Search ───────────────────────────────────────────────────────
    Task<PaginatedList<UserCardSummary>> SearchUsersAsync(
        string? currentUserId, RequestFilters filters, CancellationToken ct = default);
}