namespace Sportiva.Contracts.Notifications;

public record NotificationListResponse(
    IReadOnlyList<NotificationResponse> Items,
    int  TotalCount,
    int  UnreadCount,
    int  PageNumber,
    int  PageSize,
    bool HasMore
);
