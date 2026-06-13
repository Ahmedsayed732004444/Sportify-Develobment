using Sportiva.Contracts.Clubs;
using Sportiva.Contracts.Common;

namespace Sportiva.Services;

public interface IClubService
{
    // ── Queries ────────────────────────────────────────────────────
    Task<Result<ClubResponse>> GetClubAsync(
        string clubId, string? currentUserId,
        CancellationToken ct = default);

    Task<PaginatedList<ClubResponse>> GetClubsAsync(
        string? currentUserId, RequestFilters filters,
        CancellationToken ct = default);

    /// <summary>Gets all clubs owned by a specific user.</summary>
    Task<PaginatedList<ClubResponse>> GetClubsByOwnerAsync(
        string ownerId, string? currentUserId, RequestFilters filters,
        CancellationToken ct = default);

    // ── Commands ───────────────────────────────────────────────────
    Task<Result<ClubResponse>> CreateClubAsync(
        string ownerId, CreateClubRequest request,
        CancellationToken ct = default);

    Task<Result<ClubResponse>> UpdateClubAsync(
        string clubId, string currentUserId, UpdateClubRequest request,
        CancellationToken ct = default);

    Task<Result> DeleteClubAsync(
        string clubId, string currentUserId,
        CancellationToken ct = default);
}
