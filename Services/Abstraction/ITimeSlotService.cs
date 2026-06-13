using Sportiva.Contracts.Common;
using Sportiva.Contracts.TimeSlots;

namespace Sportiva.Services;

public interface ITimeSlotService
{
    // ── Queries ────────────────────────────────────────────────────
    Task<Result<TimeSlotResponse>> GetTimeSlotAsync(
        string timeSlotId, CancellationToken ct = default);

    Task<PaginatedList<TimeSlotResponse>> GetTimeSlotsByCourtAsync(
        string courtId, RequestFilters filters,
        CancellationToken ct = default);

    /// <summary>
    /// Returns only available (not booked) slots for a court,
    /// optionally filtered by date. Used by booking flow.
    /// </summary>
    Task<PaginatedList<TimeSlotResponse>> GetAvailableTimeSlotsAsync(
        string courtId, DateOnly? day, RequestFilters filters,
        CancellationToken ct = default);

    // ── Commands ───────────────────────────────────────────────────
    Task<Result<TimeSlotResponse>> CreateTimeSlotAsync(
        string currentUserId, CreateTimeSlotRequest request,
        CancellationToken ct = default);

    Task<Result> DeleteTimeSlotAsync(
        string timeSlotId, string currentUserId,
        CancellationToken ct = default);
}
