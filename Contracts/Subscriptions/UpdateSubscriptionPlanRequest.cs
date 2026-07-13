namespace Sportiva.Contracts.Subscriptions;

public record UpdateSubscriptionPlanRequest(
    string  Name,
    string? Description,
    decimal Price,
    int     MaxCourts,
    bool    IsActive
);
