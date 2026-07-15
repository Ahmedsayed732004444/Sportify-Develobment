using Sportiva.Contracts.TimeSlots;

namespace Sportiva.Services;

public interface ITimeSlotService
{
    Task<Result<IReadOnlyList<TimeSlotResponse>>> GetTimeSlotsAsync(
        string courtId, DateOnly? date = null, bool? available = null, CancellationToken ct = default);

    Task<Result<TimeSlotResponse>> GetTimeSlotAsync(
        string courtId, string slotId, CancellationToken ct = default);

    Task<Result<TimeSlotResponse>> CreateTimeSlotAsync(
        string userId, string courtId, CreateTimeSlotRequest request, CancellationToken ct = default);

    Task<Result<IReadOnlyList<TimeSlotResponse>>> BulkCreateTimeSlotsAsync(
        string userId, string courtId, IReadOnlyList<CreateTimeSlotRequest> requests, CancellationToken ct = default);

    Task<Result<TimeSlotResponse>> UpdateTimeSlotAsync(
        string userId, string courtId, string slotId, CreateTimeSlotRequest request, CancellationToken ct = default);

    Task<Result> DeleteTimeSlotAsync(
        string userId, string courtId, string slotId, CancellationToken ct = default);
}
