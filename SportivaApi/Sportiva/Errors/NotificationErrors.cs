namespace Sportiva.Errors;

public record NotificationErrors
{
    public static readonly Error NotFound =
        new("Notifications.NotFound", "The specified notification was not found", StatusCodes.Status404NotFound);
}
