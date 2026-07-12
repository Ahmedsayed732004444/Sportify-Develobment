namespace Sportiva.Contracts.Users;

public record ToggleFollowResponse(
 string TargetUserId,
 bool IsNowFollowing,
 int FollowersCount
);
