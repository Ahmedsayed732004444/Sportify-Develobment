namespace Sportiva.Errors;

public record ClubSubscriptionErrors
{
    public static readonly Error Error =
        new("ClubSubscriptions.Error", "An error occurred while processing the club subscription", StatusCodes.Status500InternalServerError);

    public static readonly Error SubscriptionNotFound =
        new("ClubSubscriptions.NotFound", "No subscription was found for this club", StatusCodes.Status404NotFound);

    public static readonly Error AlreadySubscribed =
        new("ClubSubscriptions.AlreadySubscribed", "This club already has an active subscription", StatusCodes.Status409Conflict);

    public static readonly Error PlanInactive =
        new("ClubSubscriptions.PlanInactive", "The specified subscription plan is not active", StatusCodes.Status400BadRequest);

    public static readonly Error NoSubscriptionToRenew =
        new("ClubSubscriptions.NoSubscriptionToRenew", "This club has no previous subscription to renew", StatusCodes.Status404NotFound);
}