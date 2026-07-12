using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Posts;

public record PostResponse(
    string      PostId,
    string      Content,
    string?     FileUrl,
    UserSummary Author,
    bool        IsOwner,
    bool        ILiked,
    int         LikesCount,
    int         CommentsCount,
    DateTime    CreatedAt
);
