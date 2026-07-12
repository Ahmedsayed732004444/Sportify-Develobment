using Microsoft.AspNetCore.Http;
using Sportiva.Abstractions;

namespace Sportiva.Errors;

public static class SubscriptionPlanErrors
{
    public static readonly Error Error =
        new("SubscriptionPlans.Error", "An error occurred while processing the subscription plan", StatusCodes.Status500InternalServerError);

    public static readonly Error PlanNotFound =
        new("SubscriptionPlans.NotFound", "The specified subscription plan was not found", StatusCodes.Status404NotFound);

    public static readonly Error DuplicateName =
        new("SubscriptionPlans.DuplicateName", "A subscription plan with the same name already exists", StatusCodes.Status400BadRequest);

    public static readonly Error InvalidName =
        new("SubscriptionPlans.InvalidName", "Plan name cannot be empty", StatusCodes.Status400BadRequest);

    public static readonly Error InvalidPrice =
        new("SubscriptionPlans.InvalidPrice", "Monthly price cannot be negative", StatusCodes.Status400BadRequest);

    public static readonly Error InvalidMaxCourts =
        new("SubscriptionPlans.InvalidMaxCourts", "Maximum courts cannot be less than zero", StatusCodes.Status400BadRequest);
}
