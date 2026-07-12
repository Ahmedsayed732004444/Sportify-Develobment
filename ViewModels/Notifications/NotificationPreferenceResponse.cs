using Sportiva.Contracts.Shared.Enums;

namespace Sportiva.Contracts.Notifications;

public record NotificationPreferenceResponse(
    NotificationTypeDto Type,
    bool                InAppEnabled,
    bool                EmailEnabled
);
