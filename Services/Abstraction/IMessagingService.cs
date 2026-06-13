using Sportiva.Contracts.Common;
using Sportiva.Contracts.Messaging;

namespace Sportiva.Services;

public interface IMessagingService
{
    Task<Result<PaginatedList<ConversationSummary>>> GetConversationsAsync(
        string userId, RequestFilters filters, CancellationToken ct = default);

    Task<Result<PaginatedList<MessageResponse>>> GetMessagesAsync(
        string userId, string otherUserId, RequestFilters filters, CancellationToken ct = default);

    Task<Result<MessageResponse>> SendMessageAsync(
        string senderId, SendMessageRequest request, CancellationToken ct = default);

    Task<Result> MarkConversationAsReadAsync(
        string userId, string otherUserId, CancellationToken ct = default);

    Task<Result> DeleteMessageAsync(
        string userId, string messageId, CancellationToken ct = default);
}
