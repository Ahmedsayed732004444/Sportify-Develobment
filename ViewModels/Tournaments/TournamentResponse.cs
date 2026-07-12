using Sportiva.Contracts.Shared.Enums;
using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Tournaments;

public record TournamentResponse(
    string              TournamentId,
    string              Name,
    string?             Description,
    SportTypeDto        SportType,
    TournamentStatusDto Status,
    DateOnly            StartDate,
    DateOnly            EndDate,
    int                 MaxParticipants,

    UserSummary Organizer,

    bool IsOwner,
    bool IParticipating,
    bool CanJoin,

    int ParticipantsCount,
    int MatchesCount,
    int CompletedMatchesCount,

    DateTime CreatedAt
);
