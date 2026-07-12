namespace Sportiva.Contracts.Tournaments;

public record CreateTournamentMatchRequest(
    string   TournamentId,
    string   Player1Id,
    string   Player2Id,
    int?     Round,
    int?     MatchNumber,
    DateOnly MatchDate,
    TimeOnly StartTime
);
