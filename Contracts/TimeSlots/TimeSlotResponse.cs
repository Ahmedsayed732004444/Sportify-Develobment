using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.TimeSlots;

public record TimeSlotResponse(
    string       TimeSlotId,
    CourtSummary Court,
    DateOnly     Day,
    TimeOnly     StartTime,
    TimeOnly     EndTime,
    bool         IsBooked,
    DateTime     CreatedAt
);
