namespace Sportiva.Contracts.Subscriptions;

public record UpdateSubscriptionPlanRequest(
    string  Name,
    string? Description,
    decimal MonthlyPrice,
    int     MaxCourts,
    bool    IsActive
);
