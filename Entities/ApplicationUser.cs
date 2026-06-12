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
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDisabled { get; set; }
    public UserProfile UserProfile { get; set; } = default!;
    public List<RefreshToken> RefreshTokens { get; set; } = [];
    public ICollection<Club> OwnedClubs { get; set; } = [];
    public ICollection<Booking> Bookings { get; set; } = [];
    public ICollection<Post> Posts { get; set; } = [];
    public ICollection<PostLike> PostLikes { get; set; } = [];
    public ICollection<MembershipUpgrade> MembershipUpgradeRequests { get; set; } = [];
    public ICollection<FriendlyMatch> OrganizedMatches { get; set; } = [];
    public ICollection<MatchJoinRequest> MatchJoinRequests { get; set; } = [];
    public ICollection<CommentReply> CommentReplies { get; set; } = [];
    public ICollection<CommentReaction> CommentReactions { get; set; } = [];
    public ICollection<ReplyReaction> ReplyReactions { get; set; } = [];
    // الناس اللي أنا بـ follow هم
    public ICollection<UserFollow> Following { get; set; } = [];

    // الناس اللي بيـ follow أنا
    public ICollection<UserFollow> Followers { get; set; } = [];
}