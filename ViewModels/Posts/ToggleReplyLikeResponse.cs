namespace Sportiva.Contracts.Posts;

public record ToggleReplyLikeResponse(
 string ReplyId,
 bool IsLiked,
 int LikesCount
);
