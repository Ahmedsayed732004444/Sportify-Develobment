using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Clubs;

public record ClubSubscriptionSummary(
    string                  SubscriptionId,
    SubscriptionPlanSummary Plan,
    DateTime                StartDate,
    DateTime                EndDate,
    bool                    IsActive
);
