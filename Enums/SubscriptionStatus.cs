namespace Sportiva.Enums;

/// <summary>
/// Represents the lifecycle state of a club subscription.
/// </summary>
public enum SubscriptionStatus
{
    /// <summary>
    /// Subscription is created but payment has not been processed yet.
    /// </summary>
    PendingPayment,

    /// <summary>
    /// Subscription is active and the subscriber has access to the club.
    /// </summary>
    Active,

    /// <summary>
    /// Subscription was cancelled by the subscriber before its natural expiration.
    /// </summary>
    Cancelled,

    /// <summary>
    /// Subscription has expired past its EndDate naturally (no user action).
    /// </summary>
    Expired
}
