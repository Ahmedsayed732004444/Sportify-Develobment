using Sportiva.Contracts.Common;
using Sportiva.Contracts.Courts;

namespace Sportiva.Services;

public interface ICourtService
{
    // ── Queries ────────────────────────────────────────────────────
    Task<Result<CourtResponse>> GetCourtAsync(
        string courtId, string? currentUserId,
        CancellationToken ct = default);

    Task<PaginatedList<CourtResponse>> GetCourtsByClubAsync(
        string clubId, string? currentUserId, RequestFilters filters,
        CancellationToken ct = default);

    Task<PaginatedList<CourtResponse>> GetCourtsAsync(
        string? currentUserId, RequestFilters filters,
        CancellationToken ct = default);

    // ── Commands ───────────────────────────────────────────────────
    Task<Result<CourtResponse>> CreateCourtAsync(
        string currentUserId, CreateCourtRequest request,
        CancellationToken ct = default);

    Task<Result<CourtResponse>> UpdateCourtAsync(
        string courtId, string currentUserId, UpdateCourtRequest request,
        CancellationToken ct = default);

    Task<Result> DeleteCourtAsync(
        string courtId, string currentUserId,
        CancellationToken ct = default);
}
