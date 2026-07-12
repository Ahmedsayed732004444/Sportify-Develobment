using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Tournaments;

public record TournamentMatchResponse(
    string       MatchId,
    string       TournamentId,
    int?         Round,
    int?         MatchNumber,
    UserSummary  Player1,
    UserSummary  Player2,
    UserSummary? Winner,
    bool         IsDecided,
    DateOnly     MatchDate,
    TimeOnly     StartTime
);
