using Sportiva.Contracts.Common;
using Sportiva.Contracts.Reviews;
using Sportiva.Contracts.Shared.Enums;
using Sportiva.Contracts.Shared.Summaries;
using Sportiva.Extensions;
using System.Linq.Expressions;

namespace Sportiva.Services;

public class ReviewService(
    ApplicationDbContext context,
    ILogger<ReviewService> logger) : IReviewService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<ReviewService> _logger = logger;

    private static readonly string[] AllowedReviewSortColumns = ["CreatedAt", "Rating"];

    // ════════════════════════════════════════════════════════════════
    //  Projection — بيتحمّل مرة واحدة ويتستخدم في كل الـ queries عشان
    //  منكررش نفس الـ Select في كل method (Court + Club + Author)
    // ════════════════════════════════════════════════════════════════

    private sealed record ReviewProjection(
        string Id,
        int Rating,
        string? Comment,
        DateTime CreatedAt,

        string AuthorId,
        string AuthorFullName,
        string? AuthorPicture,

        string CourtId,
        string? CourtName,
        string? CourtImageUrl,
        SportType CourtSportType,
        decimal CourtPricePerHour,

        string ClubId,
        string? ClubName,
        string? ClubLogoUrl,
        string? ClubCity,
        string? ClubGovernorate
    );

    private static readonly Expression<Func<Review, ReviewProjection>> ToProjection = r => new ReviewProjection(
        r.Id, r.Rating, r.Comment, r.CreatedAt,

        r.UserId,
        r.User.FullName,
        r.User.UserProfile == null ? null : r.User.UserProfile.ProfilePictureUrl,

        r.CourtId,
        r.Court.Name,
        r.Court.ImageUrl,
        r.Court.SportType,
        r.Court.PricePerHour,

        r.Court.ClubId,
        r.Court.Club.Name,
        r.Court.Club.LogoUrl,
        r.Court.Club.City,
        r.Court.Club.Governorate
    );

    // ════════════════════════════════════════════════════════════════
    //  Get Single Review
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<ReviewResponse>> GetReviewAsync(
        string reviewId, string? currentUserId = null, CancellationToken ct = default)
    {
        try
        {
            var review = await _context.Reviews
                .Where(r => r.Id == reviewId)
                .Select(ToProjection)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (review is null)
                return Result.Failure<ReviewResponse>(ReviewErrors.ReviewNotFound);

            return Result.Success(ToResponse(review, currentUserId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving review {ReviewId}", reviewId);
            return Result.Failure<ReviewResponse>(ReviewErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Get My Review For a Specific Booking
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<ReviewResponse>> GetBookingReviewAsync(
        string userId, string bookingId, CancellationToken ct = default)
    {
        try
        {
            var review = await _context.Reviews
                .Where(r => r.BookingId == bookingId && r.UserId == userId)
                .Select(ToProjection)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (review is null)
                return Result.Failure<ReviewResponse>(ReviewErrors.ReviewNotFound);

            return Result.Success(ToResponse(review, userId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving review for booking {BookingId}", bookingId);
            return Result.Failure<ReviewResponse>(ReviewErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Get Court Reviews (public — كل الـ reviews بتاعة ملعب معين)
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<PaginatedList<ReviewResponse>>> GetCourtReviewsAsync(
        string courtId, string? currentUserId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var courtExists = await _context.Courts.AnyAsync(c => c.Id == courtId && !c.IsDeleted, ct);

            if (!courtExists)
                return Result.Failure<PaginatedList<ReviewResponse>>(CourtErrors.CourtNotFound);

            var query = _context.Reviews
                .Where(r => r.CourtId == courtId)
                .ApplyFilters(filters, allowedSortColumns: AllowedReviewSortColumns)
                .Select(ToProjection);

            var paged = await query.AsNoTracking().ToPaginatedListAsync(filters, ct);
            var result = paged.Select(r => ToResponse(r, currentUserId));

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving reviews for court {CourtId}", courtId);
            return Result.Failure<PaginatedList<ReviewResponse>>(ReviewErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Get Club Reviews (public — كل الـ reviews بتاعة ملاعب النادي كله)
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<PaginatedList<ReviewResponse>>> GetClubReviewsAsync(
        string clubId, string? currentUserId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var clubExists = await _context.Clubs.AnyAsync(c => c.Id == clubId && !c.IsDeleted, ct);

            if (!clubExists)
                return Result.Failure<PaginatedList<ReviewResponse>>(ClubErrors.ClubNotFound);

            var query = _context.Reviews
                .Where(r => r.Court.ClubId == clubId)
                .ApplyFilters(filters, allowedSortColumns: AllowedReviewSortColumns)
                .Select(ToProjection);

            var paged = await query.AsNoTracking().ToPaginatedListAsync(filters, ct);
            var result = paged.Select(r => ToResponse(r, currentUserId));

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving reviews for club {ClubId}", clubId);
            return Result.Failure<PaginatedList<ReviewResponse>>(ReviewErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Get My Reviews (كل الـ reviews اللي أنا كتبتها)
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<PaginatedList<ReviewResponse>>> GetMyReviewsAsync(
        string userId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var query = _context.Reviews
                .Where(r => r.UserId == userId)
                .ApplyFilters(filters, allowedSortColumns: AllowedReviewSortColumns)
                .Select(ToProjection);

            var paged = await query.AsNoTracking().ToPaginatedListAsync(filters, ct);
            var result = paged.Select(r => ToResponse(r, userId));

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving reviews for user {UserId}", userId);
            return Result.Failure<PaginatedList<ReviewResponse>>(ReviewErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Create Review
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<ReviewResponse>> CreateReviewAsync(
        string userId, CreateReviewRequest request, CancellationToken ct = default)
    {
        try
        {
            if (request.Rating is < 1 or > 5)
                return Result.Failure<ReviewResponse>(ReviewErrors.InvalidRating);

            var booking = await _context.Bookings
                .Where(b => b.Id == request.BookingId && !b.IsDeleted)
                .Select(b => new { b.Id, b.UserId, b.CourtId, b.Status })
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (booking is null)
                return Result.Failure<ReviewResponse>(ReviewErrors.BookingNotFound);

            if (booking.UserId != userId)
                return Result.Failure<ReviewResponse>(ReviewErrors.BookingNotYours);

            if (booking.Status != BookingStatus.Completed)
                return Result.Failure<ReviewResponse>(ReviewErrors.BookingNotCompleted);

            var alreadyReviewed = await _context.Reviews
                .AnyAsync(r => r.BookingId == booking.Id, ct);

            if (alreadyReviewed)
                return Result.Failure<ReviewResponse>(ReviewErrors.AlreadyReviewed);

            var review = new Review
            {
                CourtId = booking.CourtId,
                UserId = userId,
                BookingId = booking.Id,
                Rating = request.Rating,
                Comment = request.Comment
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync(ct);

            var created = await _context.Reviews
                .Where(r => r.Id == review.Id)
                .Select(ToProjection)
                .AsNoTracking()
                .FirstAsync(ct);

            return Result.Success(ToResponse(created, userId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating review for booking {BookingId}", request.BookingId);
            return Result.Failure<ReviewResponse>(ReviewErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Update Review — بيتم تعديل الـ Rating/Comment بس.
    //  ملحوظة: request.BookingId بيتم تجاهله تماماً هنا؛ الـ review فاضل
    //  مربوط بنفس الـ booking الأصلي بتاعه ومينفعش يتغير.
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<ReviewResponse>> UpdateReviewAsync(
        string userId, string reviewId, CreateReviewRequest request, CancellationToken ct = default)
    {
        try
        {
            if (request.Rating is < 1 or > 5)
                return Result.Failure<ReviewResponse>(ReviewErrors.InvalidRating);

            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.Id == reviewId, ct);

            if (review is null)
                return Result.Failure<ReviewResponse>(ReviewErrors.ReviewNotFound);

            if (review.UserId != userId)
                return Result.Failure<ReviewResponse>(ReviewErrors.Unauthorized);

            review.Rating = request.Rating;
            review.Comment = request.Comment;

            await _context.SaveChangesAsync(ct);

            var updated = await _context.Reviews
                .Where(r => r.Id == review.Id)
                .Select(ToProjection)
                .AsNoTracking()
                .FirstAsync(ct);

            return Result.Success(ToResponse(updated, userId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating review {ReviewId}", reviewId);
            return Result.Failure<ReviewResponse>(ReviewErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Delete Review (soft delete)
    // ════════════════════════════════════════════════════════════════

    public async Task<Result> DeleteReviewAsync(
        string userId, string reviewId, CancellationToken ct = default)
    {
        try
        {
            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.Id == reviewId, ct);

            if (review is null)
                return Result.Failure(ReviewErrors.ReviewNotFound);

            if (review.UserId != userId)
                return Result.Failure(ReviewErrors.Unauthorized);

            review.IsDeleted = true;
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting review {ReviewId}", reviewId);
            return Result.Failure(ReviewErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Helpers
    // ════════════════════════════════════════════════════════════════

    private static ReviewResponse ToResponse(ReviewProjection r, string? currentUserId) => new(
        ReviewId: r.Id,
        Rating: r.Rating,
        Comment: r.Comment,
        Court: new CourtSummary(
            r.CourtId, r.CourtName, r.CourtImageUrl,
            (SportTypeDto)(int)r.CourtSportType, r.CourtPricePerHour,
            new ClubSummary(r.ClubId, r.ClubName, r.ClubLogoUrl, r.ClubCity, r.ClubGovernorate)),
        Author: new UserSummary(r.AuthorId, r.AuthorFullName, r.AuthorPicture),
        IsOwner: currentUserId is not null && r.AuthorId == currentUserId,
        CreatedAt: r.CreatedAt
    );
}