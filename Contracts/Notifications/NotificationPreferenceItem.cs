using Sportiva.Contracts.Shared.Enums;

namespace Sportiva.Contracts.Notifications;

public record NotificationPreferenceItem(
    NotificationTypeDto Type,
    bool                InAppEnabled,
    bool                EmailEnabled
);
