namespace Sportiva.Contracts.Posts;

public record CreateReplyRequest(
    string CommentId,
    string Content
);
