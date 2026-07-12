using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Clubs;

public record ClubResponse(
    string  ClubId,
    string? Name,
    string? LogoUrl,
    string? Governorate,
    string? City,
    string? Address,
    string? PhoneNumber,
    string? Email,
    bool    IsActive,

    UserSummary Owner,

    bool IsOwner,
    bool CanManageCourts,

    int    CourtsCount,
    int    ReviewsCount,
    double AverageRating,

    ClubSubscriptionSummary? ActiveSubscription,

    DateTime CreatedAt
);
