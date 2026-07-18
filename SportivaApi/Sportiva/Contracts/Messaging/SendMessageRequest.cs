namespace Sportiva.Contracts.Messaging;

public record SendMessageRequest(
    string ReceiverId,
    string Content
);
