namespace Sportiva.Contracts.Shared.Summaries;

public record SubscriptionPlanSummary(
    string PlanId,
    string Name,
    decimal Price,
    int MaxCourts
);