using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Messaging;

public record ConversationSummary(
    UserSummary OtherParty,
    string      LastMessagePreview,
    bool        LastMessageIsMine,
    int         UnreadCount,
    DateTime    LastMessageAt
);
