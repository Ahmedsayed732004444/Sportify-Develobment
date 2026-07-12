namespace Sportiva.Contracts.Subscriptions;

public record CreateClubSubscriptionRequest(
    string ClubId,
    string PlanId,
    DateTime StartDate,
    DateTime EndDate,
    string? Name = null,
    string? Description = null,
    decimal MonthlyPrice = 0.0m,
    int MaxCourts = 5
);
