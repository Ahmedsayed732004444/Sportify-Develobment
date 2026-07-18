namespace Sportiva.Contracts.Subscriptions;

public record SubscriptionPlanResponse(
    string PlanId,
    string Name,
    string? Description,
    decimal Price,
    int MaxCourts,
    int DurationInDays,
    bool IsActive,
    DateTime? ExpiresAt,
    DateTime CreatedAt
);