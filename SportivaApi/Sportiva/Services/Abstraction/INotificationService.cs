using Sportiva.Contracts.Notifications;

namespace Sportiva.Services;

public interface INotificationService
{
    Task<Result<NotificationListResponse>> GetNotificationsAsync(
        string userId, int pageNumber, int pageSize, CancellationToken ct = default);

    Task<Result<NotificationCountersResponse>> GetNotificationCountersAsync(
        string userId, CancellationToken ct = default);

    Task<Result> MarkAsReadAsync(
        string userId, string notificationId, CancellationToken ct = default);

    Task<Result> MarkAllAsReadAsync(
        string userId, CancellationToken ct = default);

    Task<Result<NotificationPreferencesListResponse>> GetPreferencesAsync(
        string userId, CancellationToken ct = default);

    Task<Result> UpdatePreferencesAsync(
        string userId, BulkUpdateNotificationPreferencesRequest request, CancellationToken ct = default);

    Task SendNotificationAsync(
        string recipientId, Sportiva.Entities.NotificationType type, string title, string body,
        string? actorId = null, string? entityType = null, string? entityId = null,
        Sportiva.Entities.NotificationPriority priority = Sportiva.Entities.NotificationPriority.Normal,
        CancellationToken ct = default);
}
