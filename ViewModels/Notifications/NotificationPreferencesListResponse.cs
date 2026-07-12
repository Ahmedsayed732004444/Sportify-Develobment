namespace Sportiva.Contracts.Notifications;

public record NotificationPreferencesListResponse(
    IReadOnlyList<NotificationPreferenceResponse> Preferences
);
