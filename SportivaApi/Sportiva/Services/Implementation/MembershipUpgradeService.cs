using Sportiva.Contracts.Common;
using Sportiva.Contracts.Memberships;
using Sportiva.Contracts.Shared.Enums;
using Sportiva.Contracts.Shared.Summaries;
using Sportiva.Extensions;
using System.Linq.Expressions;

namespace Sportiva.Services;

public class MembershipUpgradeService(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    ILogger<MembershipUpgradeService> logger) : IMembershipUpgradeService
{
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ILogger<MembershipUpgradeService> _logger = logger;

    private static readonly string[] AllowedSortColumns = ["RequestedAt", "Status"];

    // ════════════════════════════════════════════════════════════════
    //  Projection
    // ════════════════════════════════════════════════════════════════

    private sealed record MembershipUpgradeProjection(
        string Id,
        RequestStatus Status,
        string FullName,
        string Phone,
        bool IsClubOwner,
        string? ClubName,
        string? Address,
        string? LocationUrl,
        string? Note,
        DateTime RequestedAt,
        DateTime? ReviewedAt,

        string UserId,
        string UserFullName,
        string? UserProfilePictureUrl
    );

    private static readonly Expression<Func<MembershipUpgrade, MembershipUpgradeProjection>> ToProjection = m => new MembershipUpgradeProjection(
        m.Id, m.Status, m.FullName, m.Phone, m.IsClubOwner,
        m.ClubName, m.Address, m.LocationUrl, m.Note,
        m.RequestedAt, m.ReviewedAt,

        m.UserId,
        m.User.FullName,
        m.User.UserProfile == null ? null : m.User.UserProfile.ProfilePictureUrl
    );

    // ════════════════════════════════════════════════════════════════
    //  Get Single Request (Admin)
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<MembershipUpgradeResponse>> GetUpgradeRequestAsync(
        string requestId, CancellationToken ct = default)
    {
        try
        {
            var request = await _context.MembershipUpgrades
                .Where(m => m.Id == requestId)
                .Select(ToProjection)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (request is null)
                return Result.Failure<MembershipUpgradeResponse>(MembershipUpgradeErrors.RequestNotFound);

            return Result.Success(ToResponse(request, currentUserId: null));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving membership upgrade request {RequestId}", requestId);
            return Result.Failure<MembershipUpgradeResponse>(MembershipUpgradeErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Get All Requests (Admin, Paged, filterable by Status)
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<PaginatedList<MembershipUpgradeResponse>>> GetUpgradeRequestsAsync(
        RequestFilters filters, RequestStatus? status = null, CancellationToken ct = default)
    {
        try
        {
            var query = _context.MembershipUpgrades.AsQueryable();

            if (status is not null)
                query = query.Where(m => m.Status == status);

            var projected = query
                .ApplyFilters(filters, allowedSortColumns: AllowedSortColumns)
                .Select(ToProjection);

            var paged = await projected.AsNoTracking().ToPaginatedListAsync(filters, ct);
            var result = paged.Select(m => ToResponse(m, currentUserId: null));

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving membership upgrade requests");
            return Result.Failure<PaginatedList<MembershipUpgradeResponse>>(MembershipUpgradeErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Get My Request (آخر طلب بعته المستخدم، أيًا كانت حالته)
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<MembershipUpgradeResponse>> GetMyUpgradeRequestAsync(
        string userId, CancellationToken ct = default)
    {
        try
        {
            var request = await _context.MembershipUpgrades
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.RequestedAt)
                .Select(ToProjection)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (request is null)
                return Result.Failure<MembershipUpgradeResponse>(MembershipUpgradeErrors.RequestNotFound);

            return Result.Success(ToResponse(request, userId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving membership upgrade request for user {UserId}", userId);
            return Result.Failure<MembershipUpgradeResponse>(MembershipUpgradeErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Submit Request — "one active request at a time"
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<MembershipUpgradeResponse>> SubmitUpgradeRequestAsync(
        string userId, CreateMembershipUpgradeRequest request, CancellationToken ct = default)
    {
        try
        {
            var hasPendingRequest = await _context.MembershipUpgrades
                .AnyAsync(m => m.UserId == userId && m.Status == RequestStatus.Pending, ct);

            if (hasPendingRequest)
                return Result.Failure<MembershipUpgradeResponse>(MembershipUpgradeErrors.AlreadyHasPendingRequest);

            var upgrade = new MembershipUpgrade
            {
                UserId = userId,
                FullName = request.FullName,
                Phone = request.Phone,
                IsClubOwner = request.IsClubOwner,
                ClubName = request.ClubName,
                Address = request.Address,
                LocationUrl = request.LocationUrl,
                Note = request.Note
            };

            _context.MembershipUpgrades.Add(upgrade);
            await _context.SaveChangesAsync(ct);

            var created = await _context.MembershipUpgrades
                .Where(m => m.Id == upgrade.Id)
                .Select(ToProjection)
                .AsNoTracking()
                .FirstAsync(ct);

            return Result.Success(ToResponse(created, userId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while submitting membership upgrade request for user {UserId}", userId);
            return Result.Failure<MembershipUpgradeResponse>(MembershipUpgradeErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Review Request (Admin) — Approve/Reject
    //  لو IsClubOwner=true واتوافق عليه، بنرفّع المستخدم لرول Owner
    // ════════════════════════════════════════════════════════════════

    public async Task<Result> ReviewUpgradeRequestAsync(
        string adminId, string requestId, ReviewMembershipUpgradeRequest request, CancellationToken ct = default)
    {
        try
        {
            var upgrade = await _context.MembershipUpgrades
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == requestId, ct);

            if (upgrade is null)
                return Result.Failure(MembershipUpgradeErrors.RequestNotFound);

            if (upgrade.Status != RequestStatus.Pending)
                return Result.Failure(MembershipUpgradeErrors.AlreadyReviewed);

            var newStatus = (RequestStatus)(int)request.NewStatus;

            if (newStatus == RequestStatus.Approved && upgrade.IsClubOwner)
            {
                var isAlreadyOwner = await _userManager.IsInRoleAsync(upgrade.User, DefaultRoles.Owner.Name);

                if (!isAlreadyOwner)
                {
                    var roleResult = await _userManager.AddToRoleAsync(upgrade.User, DefaultRoles.Owner.Name);

                    if (!roleResult.Succeeded)
                        return Result.Failure(MembershipUpgradeErrors.Error);
                }
            }

            upgrade.Status = newStatus;
            upgrade.ReviewedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while reviewing membership upgrade request {RequestId}", requestId);
            return Result.Failure(MembershipUpgradeErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Helpers
    // ════════════════════════════════════════════════════════════════

    private static MembershipUpgradeResponse ToResponse(MembershipUpgradeProjection m, string? currentUserId) => new(
        RequestId: m.Id,
        Status: (RequestStatusDto)(int)m.Status,
        FullName: m.FullName,
        Phone: m.Phone,
        IsClubOwner: m.IsClubOwner,
        ClubName: m.ClubName,
        Address: m.Address,
        LocationUrl: m.LocationUrl,
        Note: m.Note,
        RequestedBy: new UserSummary(m.UserId, m.UserFullName, m.UserProfilePictureUrl),
        IsMine: currentUserId is not null && m.UserId == currentUserId,
        CreatedAt: m.RequestedAt,
        ReviewedAt: m.ReviewedAt
    );
}