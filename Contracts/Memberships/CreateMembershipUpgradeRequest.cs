namespace Sportiva.Contracts.Memberships;

public record CreateMembershipUpgradeRequest(
    string  FullName,
    string  Phone,
    bool    IsClubOwner,
    string? ClubName,
    string? Address,
    string? LocationUrl,
    string? Note
);
