namespace Sportiva.Errors;

public record SubscriptionPlanErrors
{
    public static readonly Error Error =
          new("SubscriptionPlan.Error", "An error occurred while processing the SubscriptionPlan", StatusCodes.Status500InternalServerError);
    public static readonly Error PlanNotFound =
       new("SubscriptionPlan.NotFound", "The specified Subscription Plan was not found", StatusCodes.Status404NotFound);
}
