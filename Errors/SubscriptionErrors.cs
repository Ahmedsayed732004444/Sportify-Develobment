using Microsoft.AspNetCore.Http;
using Sportiva.Abstractions;

namespace Sportiva.Errors;

public record SubscriptionErrors
{
    public static readonly Error Error =
        new("Subscriptions.Error", "An error occurred while processing the subscription request", StatusCodes.Status500InternalServerError);

    public static readonly Error PlanNotFound =
        new("Subscriptions.PlanNotFound", "The specified subscription plan was not found", StatusCodes.Status404NotFound);

    public static readonly Error PlanInactive =
        new("Subscriptions.PlanInactive", "The specified subscription plan is not active", StatusCodes.Status400BadRequest);

    public static readonly Error InvalidPlan =
        new("Subscriptions.InvalidPlan", "Monthly price cannot be negative and max courts must be greater than zero", StatusCodes.Status400BadRequest);

    public static readonly Error InvalidDateRange =
        new("Subscriptions.InvalidDateRange", "End date must be after start date", StatusCodes.Status400BadRequest);

    public static readonly Error SubscriptionNotFound =
        new("Subscriptions.NotFound", "No active subscription was found for the specified club", StatusCodes.Status404NotFound);

    public static readonly Error ActiveSubscriptionExists =
        new("Subscriptions.AlreadyActive", "This club already has an active subscription", StatusCodes.Status400BadRequest);

    public static readonly Error NoSubscriptionToRenew =
        new("Subscriptions.NoSubscriptionToRenew", "This club has no previous subscription to renew", StatusCodes.Status400BadRequest);
}
