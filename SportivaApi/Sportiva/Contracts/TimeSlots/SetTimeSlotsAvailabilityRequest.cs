namespace Sportiva.Contracts.TimeSlots;

public record SetTimeSlotsAvailabilityRequest(
    IReadOnlyList<string> SlotIds,
    bool IsActive
);