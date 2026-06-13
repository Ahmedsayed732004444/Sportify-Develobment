using Sportiva.Contracts.Bookings;
using Sportiva.Contracts.Common;

namespace Sportiva.Services;

public interface IBookingService
{
    // ── Queries ────────────────────────────────────────────────────
    Task<Result<BookingResponse>> GetBookingAsync(
        string bookingId, string currentUserId,
        CancellationToken ct = default);

    Task<PaginatedList<BookingResponse>> GetMyBookingsAsync(
        string userId, RequestFilters filters,
        CancellationToken ct = default);

    /// <summary>Admin / club-owner view of all bookings for a court.</summary>
    Task<PaginatedList<BookingResponse>> GetBookingsByCourtAsync(
        string courtId, string currentUserId, RequestFilters filters,
        CancellationToken ct = default);

    // ── Commands ───────────────────────────────────────────────────
    /// <summary>
    /// Creates a booking (status = Pending). Marks the TimeSlot as booked.
    /// </summary>
    Task<Result<BookingResponse>> CreateBookingAsync(
        string userId, CreateBookingRequest request,
        CancellationToken ct = default);

    /// <summary>Club owner confirms a pending booking → Confirmed.</summary>
    Task<Result<BookingResponse>> ConfirmBookingAsync(
        string bookingId, string currentUserId,
        CancellationToken ct = default);

    /// <summary>User or club owner cancels a booking → Cancelled.</summary>
    Task<Result> CancelBookingAsync(
        string bookingId, string currentUserId,
        CancellationToken ct = default);

    /// <summary>
    /// Marks a confirmed booking as Completed (e.g. scheduled job after EndTime).
    /// </summary>
    Task<Result> CompleteBookingAsync(
        string bookingId, CancellationToken ct = default);
}
