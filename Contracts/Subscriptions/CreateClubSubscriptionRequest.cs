namespace Sportiva.Contracts.Subscriptions;

public record CreateClubSubscriptionRequest(
    string   ClubId,
    string   PlanId,
    DateTime StartDate,
    DateTime EndDate
);
