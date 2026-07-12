using Sportiva.Contracts.Shared.Enums;

namespace Sportiva.Contracts.Shared.Summaries;

public record FriendlyMatchSummary(
    string       MatchId,
    DateOnly     Date,
    TimeOnly     StartTime,
    TimeOnly     EndTime,
    SportTypeDto SportType,
    CourtSummary Court
);
