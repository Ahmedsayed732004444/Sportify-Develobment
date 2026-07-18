namespace Sportiva.Contracts.Shared.Summaries;

public record UserCardSummary(
    string UserId,
    string FullName,
    string? ProfilePictureUrl,
    string? Bio,
    string? City,
    bool IsFollowing,
    bool IsMe,
    DateTime? FollowedAt,        // null when !IsFollowing
    bool? IsDisabled = null
);
