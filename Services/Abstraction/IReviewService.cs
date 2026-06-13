using Sportiva.Contracts.Common;
using Sportiva.Contracts.Reviews;

namespace Sportiva.Services;

public interface IReviewService
{
    // ── Queries ────────────────────────────────────────────────────
    Task<Result<ReviewResponse>> GetReviewAsync(
        string reviewId, string? currentUserId,
        CancellationToken ct = default);

    Task<PaginatedList<ReviewResponse>> GetReviewsByCourtAsync(
        string courtId, string? currentUserId, RequestFilters filters,
        CancellationToken ct = default);

    Task<PaginatedList<ReviewResponse>> GetReviewsByClubAsync(
        string clubId, string? currentUserId, RequestFilters filters,
        CancellationToken ct = default);

    Task<PaginatedList<ReviewResponse>> GetMyReviewsAsync(
        string userId, RequestFilters filters,
        CancellationToken ct = default);

    // ── Commands ───────────────────────────────────────────────────
    /// <summary>
    /// Creates a review tied to a completed booking.
    /// Enforces one-review-per-booking and CanReview rules.
    /// </summary>
    Task<Result<ReviewResponse>> CreateReviewAsync(
        string userId, CreateReviewRequest request,
        CancellationToken ct = default);

    Task<Result> DeleteReviewAsync(
        string reviewId, string currentUserId,
        CancellationToken ct = default);
}
