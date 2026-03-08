namespace Sportiva.Entities;
public sealed class ApplicationUser : IdentityUser
{
    public ApplicationUser()
    {
        Id = Guid.CreateVersion7().ToString();
        SecurityStamp = Guid.CreateVersion7().ToString();
    }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public bool IsDisabled { get; set; }

    public List<RefreshToken> RefreshTokens { get; set; } = [];
    public ICollection<Club> OwnedClubs { get; set; } = [];
    public ICollection<Booking> Bookings { get; set; } = [];
    public ICollection<MembershipUpgrade> MembershipUpgradeRequests { get; set; } = [];
    public ICollection<FriendlyMatch> OrganizedMatches { get; set; } = [];
    public ICollection<MatchJoinRequest> MatchJoinRequests { get; set; } = [];
}