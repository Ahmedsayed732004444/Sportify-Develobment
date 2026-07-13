namespace Sportiva.Contracts.Subscriptions;

public record CreateSubscriptionPlanRequest(
    string  Name,
    string? Description,
    decimal Price,
    int     MaxCourts
);
