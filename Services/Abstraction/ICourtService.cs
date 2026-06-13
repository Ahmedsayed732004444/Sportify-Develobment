using Sportiva.Contracts.Common;
using Sportiva.Contracts.Courts;
using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Services;

public interface ICourtService
{
    Task<Result<PaginatedList<CourtResponse>>> SearchCourtsAsync(
        string? currentUserId, RequestFilters filters,
        SportType? sport = null, string? city = null, DateOnly? date = null,
        CancellationToken ct = default);

    Task<Result<PaginatedList<CourtResponse>>> GetClubCourtsAsync(
        string clubId, string? currentUserId, RequestFilters filters, CancellationToken ct = default);

    Task<Result<CourtResponse>> GetCourtAsync(
        string clubId, string courtId, string? currentUserId = null, CancellationToken ct = default);

    Task<Result<CourtResponse>> CreateCourtAsync(
        string userId, string clubId, CreateCourtRequest request, CancellationToken ct = default);

    Task<Result<CourtResponse>> UpdateCourtAsync(
        string userId, string clubId, string courtId, UpdateCourtRequest request, CancellationToken ct = default);
    //soft delete 
    Task<Result> DeleteCourtAsync(
        string userId, string clubId, string courtId, CancellationToken ct = default);

    Task<Result> ToggleCourtStatusAsync(
        string userId, string clubId, string courtId, CancellationToken ct = default);

    Task<Result<IReadOnlyList<TimeSlotSummary>>> GetCourtAvailabilityAsync(
        string courtId, DateOnly date, CancellationToken ct = default);
}
