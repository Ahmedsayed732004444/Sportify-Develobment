namespace Sportiva.Contracts.Posts;

public record CreateCommentRequest(
    string PostId,
    string Content
);
