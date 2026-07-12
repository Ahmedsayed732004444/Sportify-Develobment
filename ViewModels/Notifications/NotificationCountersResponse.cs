namespace Sportiva.Contracts.Notifications;

public record NotificationCountersResponse(
    int UnreadCount,
    int TotalCount
);
