namespace Sportiva.Errors;

public record ClubErrors
{
    public static readonly Error Error =
        new("Clubs.Error", "An error occurred while processing the club", StatusCodes.Status500InternalServerError);

    public static readonly Error ClubNotFound =
        new("Clubs.NotFound", "The specified club was not found", StatusCodes.Status404NotFound);

    public static readonly Error Unauthorized =
        new("Clubs.Unauthorized", "You are not authorized to manage this club", StatusCodes.Status403Forbidden);
}