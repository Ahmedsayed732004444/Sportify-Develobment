using Sportiva.Contracts.Shared.Enums;

namespace Sportiva.Contracts.Tournaments;

public record CreateTournamentRequest(
    string       Name,
    string?      Description,
    SportTypeDto SportType,
    DateOnly     StartDate,
    DateOnly     EndDate,
    int          MaxParticipants
);
