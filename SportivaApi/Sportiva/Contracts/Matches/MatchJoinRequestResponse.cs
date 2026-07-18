using Sportiva.Contracts.Shared.Enums;
using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Matches;

public record MatchJoinRequestResponse(
    string               RequestId,
    JoinRequestStatusDto Status,
    UserSummary          Player,
    FriendlyMatchSummary Match,
    bool                 IsMine,
    DateTime             CreatedAt
);
