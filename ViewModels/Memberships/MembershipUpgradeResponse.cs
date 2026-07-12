using Sportiva.Contracts.Shared.Enums;
using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Memberships;

public record MembershipUpgradeResponse(
    string           RequestId,
    RequestStatusDto Status,
    string           FullName,
    string           Phone,
    bool             IsClubOwner,
    string?          ClubName,
    string?          Address,
    string?          LocationUrl,
    string?          Note,
    UserSummary      RequestedBy,
    bool             IsMine,
    DateTime         CreatedAt,
    DateTime?        ReviewedAt
);
