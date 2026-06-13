namespace Sportiva.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
{
    public DbSet<UserProfile> UserProfiles { get; set; } = default!;
    public DbSet<Post> Posts { get; set; } = default!;
    public DbSet<PostLike> PostLikes { get; set; } = default!;
    public DbSet<Club> Clubs { get; set; } = default!;
    public DbSet<Court> Courts { get; set; } = default!;
    public DbSet<TimeSlot> TimeSlots { get; set; } = default!;
    public DbSet<Booking> Bookings { get; set; } = default!;
    public DbSet<Review> Reviews { get; set; } = default!;
    public DbSet<FriendlyMatch> FriendlyMatches { get; set; } = default!;
    public DbSet<MatchJoinRequest> MatchJoinRequests { get; set; } = default!;
    public DbSet<ClubSubscription> ClubSubscriptions { get; set; } = default!;
    public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; } = default!;
    public DbSet<MembershipUpgrade> MembershipUpgrades { get; set; } = default!;
    public DbSet<Message> Messages { get; set; } = default!;
    public DbSet<Notification> Notifications { get; set; } = default!;
    public DbSet<NotificationPreference> NotificationPreferences { get; set; } = default!;
    public DbSet<Tournament> Tournaments { get; set; } = default!;
    public DbSet<TournamentMatch> TournamentMatches { get; set; } = default!;
    public DbSet<TournamentParticipant> TournamentParticipants { get; set; } = default!;
    public DbSet<PostComment> PostComments { get; set; } = default!;
    public DbSet<CommentReaction> CommentReactions { get; set; } = default!;
    public DbSet<CommentReply> CommentReplies { get; set; } = default!;
    public DbSet<ReplyReaction> ReplyReactions { get; set; } = default!;
    public DbSet<UserFollow> UserFollows { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        var cascadeFKs = modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade && !fk.IsOwnership);

        foreach (var fk in cascadeFKs)
            fk.DeleteBehavior = DeleteBehavior.Restrict;

        base.OnModelCreating(modelBuilder);
    }
}
