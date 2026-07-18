using Microsoft.AspNetCore.SignalR;
using Sportiva.Contracts.Common;
using Sportiva.Contracts.Messaging;
using Sportiva.Contracts.Shared.Summaries;
using Sportiva.Hubs;
using Sportiva.Abstractions;

namespace Sportiva.Services;

public class MessagingService(
    ApplicationDbContext context,
    IHubContext<ChatHub> chatHubContext,
    ILogger<MessagingService> logger) : IMessagingService
{
    private readonly ApplicationDbContext _context = context;
    private readonly IHubContext<ChatHub> _chatHubContext = chatHubContext;
    private readonly ILogger<MessagingService> _logger = logger;

    public async Task<Result<PaginatedList<ConversationSummary>>> GetConversationsAsync(
        string userId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var messagesQuery = _context.Messages
                .Where(m => m.SenderId == userId || m.ReceiverId == userId);

            // Fetch partner IDs as IQueryable
            var partnerIdsQuery = messagesQuery
                .Select(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Distinct();

            // Fetch the partners' users from the database as IQueryable
            var partnersQuery = _context.Users
                .Include(u => u.UserProfile)
                .Where(u => partnerIdsQuery.Contains(u.Id));

            // Order the partners by the date/time of the last message exchanged
            var orderedPartnersQuery = partnersQuery
                .OrderByDescending(partner => _context.Messages
                    .Where(m => (m.SenderId == userId && m.ReceiverId == partner.Id) ||
                                (m.SenderId == partner.Id && m.ReceiverId == userId))
                    .Max(m => m.SentAt));

            // Paginate partners
            var paginatedPartners = await PaginatedList<ApplicationUser>.CreateAsync(
                orderedPartnersQuery, filters.PageNumber, filters.PageSize, ct);

            var conversationsMap = new Dictionary<string, ConversationSummary>();

            // Sequentially load details for the current page items
            foreach (var partner in paginatedPartners.Items)
            {
                var lastMessage = await _context.Messages
                    .Where(m => (m.SenderId == userId && m.ReceiverId == partner.Id) ||
                                (m.SenderId == partner.Id && m.ReceiverId == userId))
                    .OrderByDescending(m => m.SentAt)
                    .FirstOrDefaultAsync(ct);

                if (lastMessage is null) continue;

                var unreadCount = await _context.Messages
                    .CountAsync(m => m.SenderId == partner.Id && m.ReceiverId == userId && !m.IsRead, ct);

                conversationsMap[partner.Id] = new ConversationSummary(
                    new UserSummary(partner.Id, partner.FullName, partner.UserProfile?.ProfilePictureUrl),
                    lastMessage.Content,
                    lastMessage.SenderId == userId,
                    unreadCount,
                    lastMessage.SentAt
                );
            }

            // Map using the public PaginatedList.Select method
            var paginatedResponse = paginatedPartners.Select(partner =>
                conversationsMap.TryGetValue(partner.Id, out var conv) ? conv : null!);

            // Remove any items that failed to load due to data drift
            paginatedResponse.Items.RemoveAll(c => c is null);

            return Result.Success(paginatedResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving conversations for user {UserId}", userId);
            return Result.Failure<PaginatedList<ConversationSummary>>(
                new Error("Messaging.Error", "An error occurred while retrieving conversations", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result<PaginatedList<MessageResponse>>> GetMessagesAsync(
        string userId, string otherUserId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var baseQuery = _context.Messages
                .Include(m => m.Sender)
                .ThenInclude(u => u.UserProfile)
                .Where(m => (m.SenderId == userId && m.ReceiverId == otherUserId) ||
                            (m.SenderId == otherUserId && m.ReceiverId == userId))
                .OrderByDescending(m => m.SentAt);

            // Project to MessageResponse IQueryable
            var projectedQuery = baseQuery
                .Select(m => new MessageResponse(
                    m.Id.ToString(),
                    m.Content,
                    new UserSummary(m.SenderId, m.Sender.FullName, m.Sender.UserProfile.ProfilePictureUrl),
                    m.SenderId == userId,
                    m.IsRead,
                    m.SentAt
                ));

            // Paginate and fetch using the public CreateAsync factory
            var paginatedList = await PaginatedList<MessageResponse>.CreateAsync(
                projectedQuery, filters.PageNumber, filters.PageSize, ct);

            return Result.Success(paginatedList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving messages history between user {UserId} and {OtherUserId}", userId, otherUserId);
            return Result.Failure<PaginatedList<MessageResponse>>(
                new Error("Messaging.Error", "An error occurred while retrieving message history", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result<MessageResponse>> SendMessageAsync(
        string senderId, SendMessageRequest request, CancellationToken ct = default)
    {
        try
        {
            var receiverExists = await _context.Users.AnyAsync(u => u.Id == request.ReceiverId, ct);
            if (!receiverExists)
                return Result.Failure<MessageResponse>(UserErrors.NotFound);

            var sender = await _context.Users
                .Include(u => u.UserProfile)
                .FirstOrDefaultAsync(u => u.Id == senderId, ct);

            if (sender is null)
                return Result.Failure<MessageResponse>(UserErrors.NotFound);

            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = request.ReceiverId,
                Content = request.Content,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            await _context.Messages.AddAsync(message, ct);
            await _context.SaveChangesAsync(ct);

            // Construct responses
            var senderSummary = new UserSummary(senderId, sender.FullName, sender.UserProfile?.ProfilePictureUrl);

            var messageResponseForSender = new MessageResponse(
                message.Id.ToString(),
                message.Content,
                senderSummary,
                IsMine: true,
                IsRead: false,
                SentAt: message.SentAt
            );

            var messageResponseForReceiver = messageResponseForSender with { IsMine = false };

            // Real-time broadcast pushes via SignalR ChatHub
            // Push to receiver
            await _chatHubContext.Clients.User(request.ReceiverId)
                .SendAsync("ReceiveMessage", messageResponseForReceiver, ct);

            // Push to sender (sync multiple tabs/sessions)
            await _chatHubContext.Clients.User(senderId)
                .SendAsync("ReceiveMessage", messageResponseForSender, ct);

            return Result.Success(messageResponseForSender);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while sending message from {SenderId} to {ReceiverId}", senderId, request.ReceiverId);
            return Result.Failure<MessageResponse>(
                new Error("Messaging.Error", "An error occurred while sending message", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result> MarkConversationAsReadAsync(
        string userId, string otherUserId, CancellationToken ct = default)
    {
        try
        {
            var unreadMessages = await _context.Messages
                .Where(m => m.SenderId == otherUserId && m.ReceiverId == userId && !m.IsRead)
                .ToListAsync(ct);

            foreach (var message in unreadMessages)
            {
                message.IsRead = true;
            }

            if (unreadMessages.Count > 0)
            {
                await _context.SaveChangesAsync(ct);
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while marking conversation as read between {UserId} and {OtherUserId}", userId, otherUserId);
            return Result.Failure(
                new Error("Messaging.Error", "An error occurred while marking conversation as read", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result> DeleteMessageAsync(
        string userId, string messageId, CancellationToken ct = default)
    {
        try
        {
            if (!int.TryParse(messageId, out var id))
            {
                return Result.Failure(
                    new Error("Messaging.InvalidId", "The provided message ID is invalid", StatusCodes.Status400BadRequest));
            }

            var message = await _context.Messages.FirstOrDefaultAsync(m => m.Id == id, ct);

            if (message is null)
            {
                return Result.Failure(
                    new Error("Messaging.NotFound", "The message was not found", StatusCodes.Status404NotFound));
            }

            if (message.SenderId != userId)
            {
                return Result.Failure(
                    new Error("Messaging.Unauthorized", "You are not authorized to delete this message", StatusCodes.Status403Forbidden));
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting message {MessageId} for user {UserId}", messageId, userId);
            return Result.Failure(
                new Error("Messaging.Error", "An error occurred while deleting the message", StatusCodes.Status500InternalServerError));
        }
    }
}
