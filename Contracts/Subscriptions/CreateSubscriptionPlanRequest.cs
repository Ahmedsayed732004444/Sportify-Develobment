namespace Sportiva.Contracts.Subscriptions;

public record CreateSubscriptionPlanRequest(
    string  Name,
    string? Description,
    decimal MonthlyPrice,
    int     MaxCourts
);
