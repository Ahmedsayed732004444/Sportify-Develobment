using Sportiva.Contracts.Common;
using Sportiva.Contracts.Subscriptions;

namespace Sportiva.Services;

public interface IClubSubscriptionService
{
    // ── Queries ────────────────────────────────────────────────────
    Task<Result<ClubSubscriptionResponse>> GetSubscriptionAsync(
        string subscriptionId, string currentUserId,
        CancellationToken ct = default);

    /// <summary>Returns all (active + historical) subscriptions for a club.</summary>
    Task<PaginatedList<ClubSubscriptionResponse>> GetClubSubscriptionsAsync(
        string clubId, string currentUserId, RequestFilters filters,
        CancellationToken ct = default);

    // ── Commands (Admin) ───────────────────────────────────────────
    Task<Result<ClubSubscriptionResponse>> CreateSubscriptionAsync(
        CreateClubSubscriptionRequest request,
        CancellationToken ct = default);

    Task<Result> CancelSubscriptionAsync(
        string subscriptionId, string currentUserId,
        CancellationToken ct = default);

    // ── Payments ───────────────────────────────────────────────────
    /// <summary>Lists all payments for a given subscription.</summary>
    Task<PaginatedList<SubscriptionPaymentSummary>> GetSubscriptionPaymentsAsync(
        string subscriptionId, string currentUserId, RequestFilters filters,
        CancellationToken ct = default);

    /// <summary>Records/confirms a payment (webhook or manual admin action).</summary>
    //Task<Result<SubscriptionPaymentSummary>> RecordPaymentAsync(
    //    string subscriptionId, RecordSubscriptionPaymentRequest request,
    //    CancellationToken ct = default);
}
