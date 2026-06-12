using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Followers;

public record UserFollowResponse(
    UserCardSummary User
);
