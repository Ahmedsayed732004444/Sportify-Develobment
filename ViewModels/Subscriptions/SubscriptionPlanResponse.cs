namespace Sportiva.Contracts.Subscriptions;

public record SubscriptionPlanResponse(
    string    PlanId,
    string    Name,
    string?   Description,
    decimal   MonthlyPrice,
    int       MaxCourts,
    bool      IsActive,
    DateTime? ExpiresAt,
    DateTime  CreatedAt
);
