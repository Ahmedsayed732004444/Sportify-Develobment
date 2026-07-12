using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Posts;

public record PostCommentResponse(
    string      CommentId,
    string      PostId,
    string      Content,
    UserSummary Author,
    bool        IsOwner,
    bool        ILiked,
    int         LikesCount,
    int         RepliesCount,
    DateTime    CreatedAt
);
