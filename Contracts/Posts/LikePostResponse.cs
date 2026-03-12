namespace Sportiva.Contracts.Posts;
public sealed record LikePostResponse(
  string PostId,
  int LikesCount
);
