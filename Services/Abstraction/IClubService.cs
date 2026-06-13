using Sportiva.Contracts.Clubs;
using Sportiva.Contracts.Common;

namespace Sportiva.Services;

public interface IClubService
{
    Task<Result<ClubResponse>> GetClubAsync(
        string clubId, string? currentUserId = null, CancellationToken ct = default);

    Task<Result<PaginatedList<ClubResponse>>> GetClubsAsync(
        string? currentUserId, RequestFilters filters, CancellationToken ct = default);

    Task<Result<ClubResponse>> CreateClubAsync(
        string ownerId, CreateClubRequest request, CancellationToken ct = default);

    Task<Result<ClubResponse>> UpdateClubAsync(
        string userId, string clubId, UpdateClubRequest request, CancellationToken ct = default);
    //soft delete
    Task<Result> DeleteClubAsync(
        string userId, string clubId, CancellationToken ct = default);

    Task<Result> ToggleClubStatusAsync(
        string userId, string clubId, CancellationToken ct = default);

    Task<Result<PaginatedList<ClubResponse>>> GetMyClubsAsync(
        string userId, RequestFilters filters, CancellationToken ct = default);
}
