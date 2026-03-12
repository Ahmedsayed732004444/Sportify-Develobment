using Microsoft.EntityFrameworkCore.Storage;
using Sportiva.Repositories;
namespace Sportiva.Persistence.Repositories;
public sealed class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    private IRepository<UserProfile>?       _userProfiles;
    private IRepository<Post>?              _posts;
    private IRepository<PostLike>?          _postLikes;
    private IRepository<Club>?              _clubs;
    private IRepository<Court>?             _courts;
    private IRepository<TimeSlot>?          _timeSlots;
    private IRepository<Booking>?           _bookings;
    private IRepository<Review>?            _reviews;
    private IRepository<FriendlyMatch>?     _friendlyMatches;
    private IRepository<MatchJoinRequest>?  _matchJoinRequests;
    private IRepository<ClubSubscription>?  _clubSubscriptions;
    private IRepository<SubscriptionPlan>?  _subscriptionPlans;
    private IRepository<MembershipUpgrade>? _membershipUpgrades;

    public IRepository<UserProfile>       UserProfiles       => _userProfiles       ??= new Repository<UserProfile>(context);
    public IRepository<Post>              Posts              => _posts              ??= new Repository<Post>(context);
    public IRepository<PostLike>          PostLikes          => _postLikes          ??= new Repository<PostLike>(context);
    public IRepository<Club>              Clubs              => _clubs              ??= new Repository<Club>(context);
    public IRepository<Court>             Courts             => _courts             ??= new Repository<Court>(context);
    public IRepository<TimeSlot>          TimeSlots          => _timeSlots          ??= new Repository<TimeSlot>(context);
    public IRepository<Booking>           Bookings           => _bookings           ??= new Repository<Booking>(context);
    public IRepository<Review>            Reviews            => _reviews            ??= new Repository<Review>(context);
    public IRepository<FriendlyMatch>     FriendlyMatches    => _friendlyMatches    ??= new Repository<FriendlyMatch>(context);
    public IRepository<MatchJoinRequest>  MatchJoinRequests  => _matchJoinRequests  ??= new Repository<MatchJoinRequest>(context);
    public IRepository<ClubSubscription>  ClubSubscriptions  => _clubSubscriptions  ??= new Repository<ClubSubscription>(context);
    public IRepository<SubscriptionPlan>  SubscriptionPlans  => _subscriptionPlans  ??= new Repository<SubscriptionPlan>(context);
    public IRepository<MembershipUpgrade> MembershipUpgrades => _membershipUpgrades ??= new Repository<MembershipUpgrade>(context);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await context.SaveChangesAsync(ct);

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default)
        => await context.Database.BeginTransactionAsync(ct);

    public async ValueTask DisposeAsync()
        => await context.DisposeAsync();
}
