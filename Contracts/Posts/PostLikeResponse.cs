namespace Sportiva.Contracts.Posts;
public sealed record PostLikeResponse(
  string UserProfileId,
  string FullName,
  string? ProfilePictureUrl,
  DateTime LikedAt
);
