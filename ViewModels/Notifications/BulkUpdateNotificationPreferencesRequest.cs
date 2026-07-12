namespace Sportiva.Contracts.Notifications;

public record BulkUpdateNotificationPreferencesRequest(
    IReadOnlyList<NotificationPreferenceItem> Preferences
);
