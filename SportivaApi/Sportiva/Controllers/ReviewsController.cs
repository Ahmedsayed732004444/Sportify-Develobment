using Sportiva.Contracts.Common;
using Sportiva.Contracts.Reviews;
using Sportiva.Extensions;
using Sportiva.Services;

namespace Sportiva.Controllers;

[Route("reviews")]
[ApiController]
[Authorize]
public class ReviewsController(IReviewService reviewService) : ControllerBase
{
    private readonly IReviewService _reviewService = reviewService;

    // ════════════════════════════════════════════════════════════════
    //  Reviews — عامة (لصاحب الـ review نفسه)
    // ════════════════════════════════════════════════════════════════

    //for all users
    // GET /reviews/{reviewId}
    [HttpGet("{reviewId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetReview(string reviewId, CancellationToken ct)
    {
        var result = await _reviewService.GetReviewAsync(reviewId, User.GetUserId(), ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    //for صاحب الـ review
    // GET /reviews/my
    [HttpGet("my")]
    public async Task<IActionResult> GetMyReviews([FromQuery] RequestFilters filters, CancellationToken ct)
    {
        var result = await _reviewService.GetMyReviewsAsync(User.GetUserId()!, filters, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    //for all authenticated users (لازم يكون صاحب الحجز + الحجز Completed)
    // POST /reviews
    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewRequest request, CancellationToken ct)
    {
        var result = await _reviewService.CreateReviewAsync(User.GetUserId()!, request, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    //for صاحب الـ review
    // PUT /reviews/{reviewId}
    [HttpPut("{reviewId}")]
    public async Task<IActionResult> UpdateReview(
        string reviewId, [FromBody] CreateReviewRequest request, CancellationToken ct)
    {
        var result = await _reviewService.UpdateReviewAsync(User.GetUserId()!, reviewId, request, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    //for صاحب الـ review
    // DELETE /reviews/{reviewId}
    [HttpDelete("{reviewId}")]
    public async Task<IActionResult> DeleteReview(string reviewId, CancellationToken ct)
    {
        var result = await _reviewService.DeleteReviewAsync(User.GetUserId()!, reviewId, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    // ════════════════════════════════════════════════════════════════
    //  Booking-scoped — absolute route زي باترن BookingsController
    // ════════════════════════════════════════════════════════════════

    //for صاحب الحجز
    // GET /bookings/{bookingId}/review
    [HttpGet("/bookings/{bookingId}/review")]
    public async Task<IActionResult> GetBookingReview(string bookingId, CancellationToken ct)
    {
        var result = await _reviewService.GetBookingReviewAsync(User.GetUserId()!, bookingId, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    // ════════════════════════════════════════════════════════════════
    //  Court/Club-scoped — absolute routes زي باترن CourtsController
    // ════════════════════════════════════════════════════════════════

    //for all users
    // GET /courts/{courtId}/reviews
    [HttpGet("/courts/{courtId}/reviews")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCourtReviews(
        string courtId, [FromQuery] RequestFilters filters, CancellationToken ct)
    {
        var result = await _reviewService.GetCourtReviewsAsync(courtId, User.GetUserId(), filters, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    //for all users
    // GET /clubs/{clubId}/reviews
    [HttpGet("/clubs/{clubId}/reviews")]
    [AllowAnonymous]
    public async Task<IActionResult> GetClubReviews(
        string clubId, [FromQuery] RequestFilters filters, CancellationToken ct)
    {
        var result = await _reviewService.GetClubReviewsAsync(clubId, User.GetUserId(), filters, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}