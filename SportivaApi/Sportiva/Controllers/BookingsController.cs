using Sportiva.Contracts.Bookings;
using Sportiva.Contracts.Common;
using Sportiva.Extensions;
using Sportiva.Services;

namespace Sportiva.Controllers;

[Route("bookings")]
[ApiController]
[Authorize]
public class BookingsController(IBookingService bookingService) : ControllerBase
{
    private readonly IBookingService _bookingService = bookingService;

    // ════════════════════════════════════════════════════════════════
    //  Bookings — عامة (لصاحب الحجز نفسه)
    // ════════════════════════════════════════════════════════════════

    //for the user اللي عمل الحجز
    // GET /bookings/my
    [HttpGet("my")]
    public async Task<IActionResult> GetMyBookings(
        [FromQuery] RequestFilters filters, [FromQuery] BookingStatus? status, CancellationToken ct)
    {
        var result = await _bookingService.GetMyBookingsAsync(User.GetUserId()!, filters, status, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    //for صاحب الحجز أو صاحب الملعب
    // GET /bookings/{bookingId}
    [HttpGet("{bookingId}")]
    public async Task<IActionResult> GetBooking(string bookingId, CancellationToken ct)
    {
        var result = await _bookingService.GetBookingAsync(bookingId, User.GetUserId()!, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    //for all authenticated users
    // POST /bookings
    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request, CancellationToken ct)
    {
        var result = await _bookingService.CreateBookingAsync(User.GetUserId()!, request, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    //for صاحب الحجز
    // DELETE /bookings/{bookingId}
    [HttpDelete("{bookingId}")]
    public async Task<IActionResult> CancelBooking(string bookingId, CancellationToken ct)
    {
        var result = await _bookingService.CancelBookingAsync(User.GetUserId()!, bookingId, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    //for صاحب الحجز
    // GET /bookings/{bookingId}/receipt
    [HttpGet("{bookingId}/receipt")]
    public async Task<IActionResult> GetBookingReceipt(string bookingId, CancellationToken ct)
    {
        var result = await _bookingService.GetBookingReceiptAsync(User.GetUserId()!, bookingId, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    //for صاحب النادي (يقبل/يرفض الحجز)
    // PATCH /bookings/{bookingId}/review
    [HttpPatch("{bookingId}/review")]
    public async Task<IActionResult> ReviewBooking(
        string bookingId, [FromBody] ReviewBookingRequest request, CancellationToken ct)
    {
        var result = await _bookingService.ReviewBookingAsync(User.GetUserId()!, bookingId, request, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    // ════════════════════════════════════════════════════════════════
    //  Court/Club-scoped — absolute routes زي باترن CourtsController
    // ════════════════════════════════════════════════════════════════

    //for صاحب الملعب
    // GET /courts/{courtId}/bookings
    [HttpGet("/courts/{courtId}/bookings")]
    public async Task<IActionResult> GetCourtBookings(
        string courtId, [FromQuery] RequestFilters filters, [FromQuery] DateOnly? date, CancellationToken ct)
    {
        var result = await _bookingService.GetCourtBookingsAsync(User.GetUserId()!, courtId, filters, date, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    //for صاحب النادي
    // GET /clubs/{clubId}/bookings
    [HttpGet("/clubs/{clubId}/bookings")]
    public async Task<IActionResult> GetClubBookings(
        string clubId, [FromQuery] RequestFilters filters, CancellationToken ct)
    {
        var result = await _bookingService.GetClubBookingsAsync(User.GetUserId()!, clubId, filters, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}