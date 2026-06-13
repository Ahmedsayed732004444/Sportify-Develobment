using Sportiva.Contracts.Common;
using Sportiva.Contracts.Notifications;

namespace Sportiva.Services;

public interface INotificationService
{
    // ── Notification Queries ───────────────────────────────────────
    Task<NotificationListResponse> GetNotificationsAsync(
        string userId, RequestFilters filters,
        CancellationToken ct = default);

    Task<NotificationCountersResponse> GetCountersAsync(
        string userId, CancellationToken ct = default);

    // ── Notification Commands ──────────────────────────────────────
    Task<Result> MarkAsReadAsync(
        string notificationId, string userId,
        CancellationToken ct = default);

    Task<Result> MarkAllAsReadAsync(
        string userId, CancellationToken ct = default);

    Task<Result> DeleteNotificationAsync(
        string notificationId, string userId,
        CancellationToken ct = default);

    // ── Preferences ────────────────────────────────────────────────
    Task<NotificationPreferencesListResponse> GetPreferencesAsync(
        string userId, CancellationToken ct = default);

    Task<Result> UpdatePreferencesAsync(
        string userId, BulkUpdateNotificationPreferencesRequest request,
        CancellationToken ct = default);

    // ── Internal (called by other services, not exposed via HTTP) ──
    /// <summary>
    /// Creates and dispatches a notification.
    /// Respects per-user NotificationPreference (InApp / Email).
    /// </summary>
    //Task SendAsync(
    //    string recipientId,
    //    string? actorId,
    //    NotificationType type,
    //    string title,
    //    string message,
    //    string? entityType = null,
    //    string? entityId   = null,
    //    CancellationToken ct = default);
}
