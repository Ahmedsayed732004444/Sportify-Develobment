using Sportiva.Contracts.Shared.Enums;
using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Notifications;

public record NotificationResponse(
    string                   NotificationId,
    NotificationTypeDto      Type,
    NotificationPriorityDto? Priority,    // optional — use for styling (e.g. SecurityAlert = High)
    string                   Title,
    string                   Body,
    UserSummary?             Actor,
    string?                  EntityType,
    string?                  EntityId,
    bool                     IsRead,
    DateTime?                ReadAt,
    DateTime                 CreatedAt
);
