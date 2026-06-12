namespace Sportiva.Contracts.Shared.Summaries;

public record SubscriptionPlanSummary(
    string  PlanId,
    string  Name,
    decimal MonthlyPrice,
    int     MaxCourts
);
