using Sportiva.Contracts.Shared.Enums;

namespace Sportiva.Contracts.Users;

public record UserProfileResponse(
    // — Identity
    string UserId,
    string FirstName,
    string LastName,
    string FullName,
    string Email,

    // — Profile
    string?       Bio,
    string?       City,
    string?       Country,
    string?       ProfilePictureUrl,
    string?       CoverImageUrl,
    SportTypeDto? PreferredSport,
    string?       PreferredCity,

    // — Current-user context
    bool IsMe,
    bool IsFollowing,
    bool CanSendMessage,

    // — Counters
    int FollowersCount,
    int FollowingCount,
    int PostsCount,

    // — Metadata
    DateTime CreatedAt
);
