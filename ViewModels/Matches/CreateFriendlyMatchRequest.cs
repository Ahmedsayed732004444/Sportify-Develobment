using Sportiva.Contracts.Shared.Enums;

namespace Sportiva.Contracts.Matches;

public record CreateFriendlyMatchRequest(
    string       CourtId,
    DateOnly     Date,
    TimeOnly     StartTime,
    TimeOnly     EndTime,
    SportTypeDto SportType,
    int          RequiredPlayers,
    string?      Note
);
