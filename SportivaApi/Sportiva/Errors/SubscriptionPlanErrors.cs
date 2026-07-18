namespace Sportiva.Errors;

public record SubscriptionPlanErrors
{
    public static readonly Error Error =
        new("SubscriptionPlans.Error", "An error occurred while processing the subscription plan", StatusCodes.Status500InternalServerError);

    public static readonly Error PlanNotFound =
        new("SubscriptionPlans.NotFound", "The specified subscription plan was not found", StatusCodes.Status404NotFound);
}