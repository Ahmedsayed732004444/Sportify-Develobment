namespace Sportiva.Contracts.TimeSlots;

public record CreateTimeSlotRequest(
    string   CourtId,
    DateOnly Day,
    TimeOnly StartTime,
    TimeOnly EndTime
);
