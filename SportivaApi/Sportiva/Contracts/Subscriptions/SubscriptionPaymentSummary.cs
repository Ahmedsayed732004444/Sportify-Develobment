using Sportiva.Contracts.Shared.Enums;

namespace Sportiva.Contracts.Subscriptions;

public record SubscriptionPaymentSummary(
    string           PaymentId,
    decimal          Amount,
    PaymentStatusDto Status,
    string?          TransactionId,
    DateTime?        PaidAt
);
