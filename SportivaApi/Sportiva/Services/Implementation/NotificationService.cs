using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Linq.Expressions;
using Sportiva.Contracts.Notifications;
using Sportiva.Contracts.Shared.Enums;
using Sportiva.Contracts.Shared.Summaries;
using Sportiva.Hubs;

namespace Sportiva.Services;

public class NotificationService(
    ApplicationDbContext context,
    IHubContext<NotificationHub> hubContext,
    IEmailSender emailSender,
    ILogger<NotificationService> logger) : INotificationService
{
    private readonly ApplicationDbContext _context = context;
    private readonly IHubContext<NotificationHub> _hubContext = hubContext;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly ILogger<NotificationService> _logger = logger;

    private static readonly Expression<Func<Notification, NotificationResponse>> ToProjection =
        n => new NotificationResponse(
            n.Id,
            (NotificationTypeDto)n.Type,
            (NotificationPriorityDto)n.Priority,
            n.Title,
            n.Message,
            n.Actor != null ? new UserSummary(n.Actor.Id, n.Actor.FullName, n.Actor.UserProfile.ProfilePictureUrl) : null,
            n.EntityType,
            n.EntityId,
            n.IsRead,
            n.ReadAt,
            n.CreatedAt
        );

    public async Task<Result<NotificationListResponse>> GetNotificationsAsync(
        string userId, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        try
        {
            var baseQuery = _context.Notifications
                .Where(n => n.RecipientId == userId);

            var totalCount = await baseQuery.CountAsync(ct);
            var unreadCount = await baseQuery.CountAsync(n => !n.IsRead, ct);

            var items = await baseQuery
                .OrderByDescending(n => n.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(ToProjection)
                .AsNoTracking()
                .ToListAsync(ct);

            var hasMore = totalCount > pageNumber * pageSize;

            var response = new NotificationListResponse(
                items,
                totalCount,
                unreadCount,
                pageNumber,
                pageSize,
                hasMore
            );

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching notifications for user {UserId}", userId);
            return Result.Failure<NotificationListResponse>(
                new Error("Notifications.Error", "An error occurred while retrieving notifications", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result<NotificationCountersResponse>> GetNotificationCountersAsync(
        string userId, CancellationToken ct = default)
    {
        try
        {
            var query = _context.Notifications.Where(n => n.RecipientId == userId);
            var unreadCount = await query.CountAsync(n => !n.IsRead, ct);
            var totalCount = await query.CountAsync(ct);

            return Result.Success(new NotificationCountersResponse(unreadCount, totalCount));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching notification counters for user {UserId}", userId);
            return Result.Failure<NotificationCountersResponse>(
                new Error("Notifications.Error", "An error occurred while retrieving notification counters", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result> MarkAsReadAsync(
        string userId, string notificationId, CancellationToken ct = default)
    {
        try
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.RecipientId == userId, ct);

            if (notification is null)
                return Result.Failure(NotificationErrors.NotFound);

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(ct);
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while marking notification {NotificationId} as read for user {UserId}", notificationId, userId);
            return Result.Failure(
                new Error("Notifications.Error", "An error occurred while updating the notification status", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result> MarkAllAsReadAsync(
        string userId, CancellationToken ct = default)
    {
        try
        {
            var unreadNotifications = await _context.Notifications
                .Where(n => n.RecipientId == userId && !n.IsRead)
                .ToListAsync(ct);

            var now = DateTime.UtcNow;
            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                notification.ReadAt = now;
            }

            if (unreadNotifications.Count > 0)
            {
                await _context.SaveChangesAsync(ct);
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while marking all notifications as read for user {UserId}", userId);
            return Result.Failure(
                new Error("Notifications.Error", "An error occurred while updating notifications", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result<NotificationPreferencesListResponse>> GetPreferencesAsync(
        string userId, CancellationToken ct = default)
    {
        try
        {
            var userPrefs = await _context.NotificationPreferences
                .Where(p => p.UserId == userId)
                .ToDictionaryAsync(p => p.Type, ct);

            var allTypes = Enum.GetValues<Sportiva.Entities.NotificationType>();
            var responseList = new List<NotificationPreferenceResponse>();

            foreach (var type in allTypes)
            {
                if (userPrefs.TryGetValue(type, out var pref))
                {
                    responseList.Add(new NotificationPreferenceResponse(
                        (NotificationTypeDto)type,
                        pref.InAppEnabled,
                        pref.EmailEnabled
                    ));
                }
                else
                {
                    // Default values if no preference is stored in the DB
                    responseList.Add(new NotificationPreferenceResponse(
                        (NotificationTypeDto)type,
                        InAppEnabled: true,
                        EmailEnabled: false
                    ));
                }
            }

            return Result.Success(new NotificationPreferencesListResponse(responseList));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching notification preferences for user {UserId}", userId);
            return Result.Failure<NotificationPreferencesListResponse>(
                new Error("Notifications.Error", "An error occurred while retrieving preferences", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result> UpdatePreferencesAsync(
        string userId, BulkUpdateNotificationPreferencesRequest request, CancellationToken ct = default)
    {
        try
        {
            var existingPrefs = await _context.NotificationPreferences
                .Where(p => p.UserId == userId)
                .ToDictionaryAsync(p => p.Type, ct);

            foreach (var item in request.Preferences)
            {
                var backendType = (Sportiva.Entities.NotificationType)item.Type;

                if (existingPrefs.TryGetValue(backendType, out var pref))
                {
                    pref.InAppEnabled = item.InAppEnabled;
                    pref.EmailEnabled = item.EmailEnabled;
                }
                else
                {
                    var newPref = new NotificationPreference
                    {
                        UserId = userId,
                        Type = backendType,
                        InAppEnabled = item.InAppEnabled,
                        EmailEnabled = item.EmailEnabled
                    };
                    await _context.NotificationPreferences.AddAsync(newPref, ct);
                }
            }

            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating notification preferences for user {UserId}", userId);
            return Result.Failure(
                new Error("Notifications.Error", "An error occurred while updating preferences", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task SendNotificationAsync(
        string recipientId, Sportiva.Entities.NotificationType type, string title, string body,
        string? actorId = null, string? entityType = null, string? entityId = null,
        Sportiva.Entities.NotificationPriority priority = Sportiva.Entities.NotificationPriority.Normal,
        CancellationToken ct = default)
    {
        try
        {
            var pref = await _context.NotificationPreferences
                .FirstOrDefaultAsync(p => p.UserId == recipientId && p.Type == type, ct);

            bool inAppEnabled = pref?.InAppEnabled ?? true;
            bool emailEnabled = pref?.EmailEnabled ?? false;

            if (!inAppEnabled && !emailEnabled)
                return;

            Notification? notification = null;

            if (inAppEnabled)
            {
                notification = new Notification
                {
                    RecipientId = recipientId,
                    ActorId = actorId,
                    Type = type,
                    Priority = priority,
                    Title = title,
                    Message = body,
                    EntityType = entityType,
                    EntityId = entityId,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Notifications.AddAsync(notification, ct);
                await _context.SaveChangesAsync(ct);

                // Push real-time over WebSockets (SignalR)
                // Load Actor with UserProfile to fulfill UserSummary
                UserSummary? actorSummary = null;
                if (actorId is not null)
                {
                    var actor = await _context.Users
                        .Include(u => u.UserProfile)
                        .FirstOrDefaultAsync(u => u.Id == actorId, ct);

                    if (actor is not null)
                    {
                        actorSummary = new UserSummary(actor.Id, actor.FullName, actor.UserProfile?.ProfilePictureUrl);
                    }
                }

                var response = new NotificationResponse(
                    notification.Id,
                    (NotificationTypeDto)type,
                    (NotificationPriorityDto)priority,
                    title,
                    body,
                    actorSummary,
                    entityType,
                    entityId,
                    IsRead: false,
                    ReadAt: null,
                    CreatedAt: notification.CreatedAt
                );

                await _hubContext.Clients.User(recipientId)
                    .SendAsync("ReceiveNotification", response, ct);
            }

            if (emailEnabled)
            {
                var recipientEmail = await _context.Users
                    .Where(u => u.Id == recipientId)
                    .Select(u => u.Email)
                    .FirstOrDefaultAsync(ct);

                if (!string.IsNullOrEmpty(recipientEmail))
                {
                    await _emailSender.SendEmailAsync(recipientEmail, title, body);

                    if (notification is not null)
                    {
                        notification.EmailSent = true;
                        await _context.SaveChangesAsync(ct);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification of type {Type} to user {RecipientId}", type, recipientId);
        }
    }
}
