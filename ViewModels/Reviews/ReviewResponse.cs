using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Reviews;

public record ReviewResponse(
    string       ReviewId,
    int          Rating,
    string?      Comment,
    CourtSummary Court,
    UserSummary  Author,
    bool         IsOwner,
    DateTime     CreatedAt
);
