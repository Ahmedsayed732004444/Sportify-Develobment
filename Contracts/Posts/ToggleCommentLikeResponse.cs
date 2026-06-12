namespace Sportiva.Contracts.Posts;

public record ToggleCommentLikeResponse(
 string CommentId,
 bool IsLiked,
 int LikesCount
);
