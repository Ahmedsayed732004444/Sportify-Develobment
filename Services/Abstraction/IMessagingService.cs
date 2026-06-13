using Sportiva.Contracts.Common;
using Sportiva.Contracts.Messaging;

namespace Sportiva.Services;

public interface IMessagingService
{
    // ── Conversations ──────────────────────────────────────────────
    /// <summary>Returns the inbox — one entry per unique conversation partner.</summary>
    Task<PaginatedList<ConversationSummary>> GetConversationsAsync(
        string userId, RequestFilters filters,
        CancellationToken ct = default);

    // ── Messages ───────────────────────────────────────────────────
    Task<PaginatedList<MessageResponse>> GetConversationAsync(
        string userId, string otherUserId, RequestFilters filters,
        CancellationToken ct = default);

    Task<Result<MessageResponse>> SendMessageAsync(
        string senderId, SendMessageRequest request,
        CancellationToken ct = default);

    /// <summary>Marks all unread messages in a conversation as read.</summary>
    Task<Result> MarkConversationAsReadAsync(
        string userId, string otherUserId,
        CancellationToken ct = default);

    Task<Result> DeleteMessageAsync(
        string messageId, string currentUserId,
        CancellationToken ct = default);
}
