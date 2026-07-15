using Sportiva.Contracts.Shared.Enums;

namespace Sportiva.Contracts.Matches;

public record ReviewJoinRequestRequest(
    JoinRequestStatusDto NewStatus
);
