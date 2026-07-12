using Sportiva.Contracts.Shared.Enums;
using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Matches;

public record FriendlyMatchResponse(
    string         MatchId,
    DateOnly       Date,
    TimeOnly       StartTime,
    TimeOnly       EndTime,
    SportTypeDto   SportType,
    int            RequiredPlayers,
    int            AcceptedPlayersCount,
    int            SlotsRemaining,
    MatchStatusDto Status,
    string?        Note,

    CourtSummary Court,
    UserSummary  Organizer,

    bool IsOwner,
    bool IParticipating,
    bool IApplied,
    bool CanJoin,

    IReadOnlyList<ParticipantSummary> ParticipantsPreview,   // capped at 5

    DateTime CreatedAt
);
