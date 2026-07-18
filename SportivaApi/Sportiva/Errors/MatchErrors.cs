namespace Sportiva.Errors;

public record MatchErrors
{
    public static readonly Error NotFound =
        new("Matches.NotFound", "The specified friendly match was not found", StatusCodes.Status404NotFound);

    public static readonly Error NotOpen =
        new("Matches.NotOpen", "This match is not open for new join requests", StatusCodes.Status409Conflict);

    public static readonly Error OrganizerCannotJoin =
        new("Matches.OrganizerCannotJoin", "As the organizer, you are already part of this match", StatusCodes.Status409Conflict);

    public static readonly Error AlreadyRequested =
        new("Matches.AlreadyRequested", "You have already sent a join request or are a participant in this match", StatusCodes.Status409Conflict);

    public static readonly Error MatchFull =
        new("Matches.MatchFull", "This friendly match has reached its player capacity", StatusCodes.Status409Conflict);

    public static readonly Error Unauthorized =
        new("Matches.Unauthorized", "You are not authorized to perform this action on this match", StatusCodes.Status403Forbidden);

    public static readonly Error JoinRequestNotFound =
        new("Matches.JoinRequestNotFound", "The specified join request was not found", StatusCodes.Status404NotFound);

    public static readonly Error NotPending =
        new("Matches.NotPending", "This join request is no longer pending", StatusCodes.Status409Conflict);
}
