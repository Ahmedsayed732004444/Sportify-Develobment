using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Subscriptions;

public record ClubSubscriptionResponse(
    string                      SubscriptionId,
    ClubSummary                 Club,
    SubscriptionPlanSummary     Plan,
    DateTime                    StartDate,
    DateTime                    EndDate,
    bool                        IsActive,
    int                         PaymentsCount,
    SubscriptionPaymentSummary? LastPayment
);
