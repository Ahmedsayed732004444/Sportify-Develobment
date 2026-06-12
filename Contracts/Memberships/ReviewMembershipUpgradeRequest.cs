using Sportiva.Contracts.Shared.Enums;

namespace Sportiva.Contracts.Memberships;

public record ReviewMembershipUpgradeRequest(
    RequestStatusDto NewStatus
);
