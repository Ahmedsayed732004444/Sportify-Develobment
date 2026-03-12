using Microsoft.EntityFrameworkCore.Storage;

namespace Sportiva.Repositories;

public interface IUnitOfWork : IAsyncDisposable
{
    IRepository<UserProfile> UserProfiles { get; }
    IRepository<Post> Posts { get; }
    IRepository<PostLike> PostLikes { get; }
    IRepository<Club> Clubs { get; }
    IRepository<Court> Courts { get; }
    IRepository<TimeSlot> TimeSlots { get; }
    IRepository<Booking> Bookings { get; }
    IRepository<Review> Reviews { get; }
    IRepository<FriendlyMatch> FriendlyMatches { get; }
    IRepository<MatchJoinRequest> MatchJoinRequests { get; }
    IRepository<ClubSubscription> ClubSubscriptions { get; }
    IRepository<SubscriptionPlan> SubscriptionPlans { get; }
    IRepository<MembershipUpgrade> MembershipUpgrades { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default);
}
