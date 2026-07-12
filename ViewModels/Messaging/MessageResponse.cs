using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Messaging;

public record MessageResponse(
    string      MessageId,
    string      Content,
    UserSummary Sender,
    bool        IsMine,
    bool        IsRead,
    DateTime    SentAt
);
