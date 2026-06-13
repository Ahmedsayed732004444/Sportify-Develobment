using Sportiva.Contracts.Bookings;
using Sportiva.Contracts.Common;
using Sportiva.Enums;

namespace Sportiva.Services;

public interface IBookingService
{
    Task<Result<BookingResponse>> GetBookingAsync(
        string bookingId, string currentUserId, CancellationToken ct = default);

    Task<Result<PaginatedList<BookingResponse>>> GetMyBookingsAsync(
        string userId, RequestFilters filters, BookingStatus? status = null, CancellationToken ct = default);

    Task<Result<PaginatedList<BookingResponse>>> GetCourtBookingsAsync(
        string userId, string courtId, RequestFilters filters, DateOnly? date = null, CancellationToken ct = default);

    Task<Result<PaginatedList<BookingResponse>>> GetClubBookingsAsync(
        string userId, string clubId, RequestFilters filters, CancellationToken ct = default);

    Task<Result<BookingResponse>> CreateBookingAsync(
        string userId, CreateBookingRequest request, CancellationToken ct = default);

    Task<Result> CancelBookingAsync(
        string userId, string bookingId, CancellationToken ct = default);

    Task<Result<BookingResponse>> GetBookingReceiptAsync(
        string userId, string bookingId, CancellationToken ct = default);
}
