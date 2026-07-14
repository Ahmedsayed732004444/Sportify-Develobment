using Sportiva.Contracts.Bookings;
using Sportiva.Contracts.Common;
using Sportiva.Enums;
using Sportiva.Extensions;
using Sportiva.Services;
using System;

namespace Sportiva.Controllers;

[ApiController]
[Authorize]
public class BookingController(IBookingService bookingService) : ControllerBase
{
    private readonly IBookingService _bookingService = bookingService;
    private const int ClientClosedRequestStatusCode = 499;

    // POST /bookings
    [HttpPost("bookings")]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request, CancellationToken ct)
    {
        try
        {
            var result = await _bookingService.CreateBookingAsync(User.GetUserId()!, request, ct);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(ClientClosedRequestStatusCode, new { message = "Request was cancelled by the client." });
        }
    }

    // DELETE /bookings/{bookingId}
    [HttpDelete("bookings/{bookingId}")]
    public async Task<IActionResult> CancelBooking(string bookingId, CancellationToken ct)
    {
        try
        {
            var result = await _bookingService.CancelBookingAsync(User.GetUserId()!, bookingId, ct);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(ClientClosedRequestStatusCode, new { message = "Request was cancelled by the client." });
        }
    }

    // GET /bookings/my
    [HttpGet("bookings/my")]
    public async Task<IActionResult> GetMyBookings([FromQuery] RequestFilters filters, [FromQuery] BookingStatus? status, CancellationToken ct)
    {
        try
        {
            var result = await _bookingService.GetMyBookingsAsync(User.GetUserId()!, filters, status, ct);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(ClientClosedRequestStatusCode, new { message = "Request was cancelled by the client." });
        }
    }

    // GET /bookings/{bookingId}
    [HttpGet("bookings/{bookingId}")]
    public async Task<IActionResult> GetBooking(string bookingId, CancellationToken ct)
    {
        try
        {
            var result = await _bookingService.GetBookingAsync(bookingId, User.GetUserId()!, ct);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(ClientClosedRequestStatusCode, new { message = "Request was cancelled by the client." });
        }
    }

    // GET /bookings/{bookingId}/receipt
    [HttpGet("bookings/{bookingId}/receipt")]
    public async Task<IActionResult> GetBookingReceipt(string bookingId, CancellationToken ct)
    {
        try
        {
            var result = await _bookingService.GetBookingReceiptAsync(User.GetUserId()!, bookingId, ct);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(ClientClosedRequestStatusCode, new { message = "Request was cancelled by the client." });
        }
    }

    // GET /clubs/{clubId}/bookings
    [HttpGet("clubs/{clubId}/bookings")]
    public async Task<IActionResult> GetClubBookings(string clubId, [FromQuery] RequestFilters filters, CancellationToken ct)
    {
        try
        {
            var result = await _bookingService.GetClubBookingsAsync(User.GetUserId()!, clubId, filters, ct);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(ClientClosedRequestStatusCode, new { message = "Request was cancelled by the client." });
        }
    }

    // GET /courts/{courtId}/bookings
    [HttpGet("courts/{courtId}/bookings")]
    public async Task<IActionResult> GetCourtBookings(string courtId, [FromQuery] RequestFilters filters, [FromQuery] DateOnly? date, CancellationToken ct)
    {
        try
        {
            var result = await _bookingService.GetCourtBookingsAsync(User.GetUserId()!, courtId, filters, date, ct);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(ClientClosedRequestStatusCode, new { message = "Request was cancelled by the client." });
        }
    }
}
