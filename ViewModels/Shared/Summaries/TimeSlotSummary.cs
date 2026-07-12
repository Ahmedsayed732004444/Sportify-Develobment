namespace Sportiva.Contracts.Shared.Summaries;

public record TimeSlotSummary(
    string   TimeSlotId,
    DateOnly Day,
    TimeOnly StartTime,
    TimeOnly EndTime,
    bool     IsBooked
);
