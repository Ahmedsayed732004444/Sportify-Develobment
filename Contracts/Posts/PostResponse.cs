namespace Sportiva.Contracts.Posts;
public sealed record PostResponse(
    string Id,
    string Content,
    string? FileUrl,
    DateTime CreatedAt,
    int LikesCount,
    bool IsLikedByMe,    
    PostAuthorSummary Author
);
