using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Posts;

public record CommentReplyResponse(
    string      ReplyId,
    string      CommentId,
    string      Content,
    UserSummary Author,
    bool        IsOwner,
    bool        ILiked,
    int         LikesCount,
    DateTime    CreatedAt
);
