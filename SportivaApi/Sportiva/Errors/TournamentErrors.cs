namespace Sportiva.Errors;

public record TournamentErrors
{
    public static readonly Error NotFound =
        new("Tournaments.NotFound", "The specified tournament was not found", StatusCodes.Status404NotFound);

    public static readonly Error AlreadyJoined =
        new("Tournaments.AlreadyJoined", "You have already joined this tournament", StatusCodes.Status409Conflict);

    public static readonly Error TournamentFull =
        new("Tournaments.TournamentFull", "The tournament has reached its capacity limit", StatusCodes.Status409Conflict);

    public static readonly Error NotJoined =
        new("Tournaments.NotJoined", "You are not a participant in this tournament", StatusCodes.Status400BadRequest);

    public static readonly Error TournamentMatchNotFound =
        new("Tournaments.MatchNotFound", "The tournament match was not found", StatusCodes.Status404NotFound);

    public static readonly Error Unauthorized =
        new("Tournaments.Unauthorized", "You are not authorized to perform this action on this tournament", StatusCodes.Status403Forbidden);

    public static readonly Error InvalidWinner =
        new("Tournaments.InvalidWinner", "The declared winner is not a participant in this match", StatusCodes.Status400BadRequest);
}
