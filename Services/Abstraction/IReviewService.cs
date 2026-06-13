using Sportiva.Contracts.Common;
using Sportiva.Contracts.Reviews;

namespace Sportiva.Services;

public interface IReviewService
{
    Task<Result<ReviewResponse>> GetReviewAsync(
        string reviewId, string? currentUserId = null, CancellationToken ct = default);

    Task<Result<ReviewResponse>> GetBookingReviewAsync(
        string userId, string bookingId, CancellationToken ct = default);

    Task<Result<PaginatedList<ReviewResponse>>> GetCourtReviewsAsync(
        string courtId, string? currentUserId, RequestFilters filters, CancellationToken ct = default);

    Task<Result<PaginatedList<ReviewResponse>>> GetClubReviewsAsync(
        string clubId, string? currentUserId, RequestFilters filters, CancellationToken ct = default);

    Task<Result<PaginatedList<ReviewResponse>>> GetMyReviewsAsync(
        string userId, RequestFilters filters, CancellationToken ct = default);

    Task<Result<ReviewResponse>> CreateReviewAsync(
        string userId, CreateReviewRequest request, CancellationToken ct = default);

    Task<Result<ReviewResponse>> UpdateReviewAsync(
        string userId, string reviewId, CreateReviewRequest request, CancellationToken ct = default);

    Task<Result> DeleteReviewAsync(
        string userId, string reviewId, CancellationToken ct = default);
}
