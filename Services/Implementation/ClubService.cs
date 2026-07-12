using Sportiva.Contracts.Clubs;
using Sportiva.Contracts.Common;
using Sportiva.Contracts.Shared.Summaries;
using Sportiva.Extensions;

namespace Sportiva.Services;

public class ClubService(
    ApplicationDbContext context,
    ILogger<ClubService> logger,
    IWebHostEnvironment env,
    IHttpContextAccessor accessor) : IClubService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<ClubService> _logger = logger;
    private readonly IWebHostEnvironment _env = env;
    private readonly IHttpContextAccessor _accessor = accessor;

    private static readonly string[] AllowedClubSortColumns = ["Name", "CreatedAt"];
    private const string LogoLocation = "uploads/clubs";

    // ════════════════════════════════════════════════════════════════
    //  Get Single Club
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<ClubResponse>> GetClubAsync(
        string clubId, string? currentUserId = null, CancellationToken ct = default)
    {
        try
        {
            var club = await _context.Clubs
                .Where(c => c.Id == clubId && !c.IsDeleted)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.LogoUrl,
                    c.Governorate,
                    c.City,
                    c.Address,
                    c.PhoneNumber,
                    c.Email,
                    c.IsActive,
                    c.OwnerId,
                    OwnerFullName = c.Owner.FullName,
                    OwnerPicture = c.Owner.UserProfile == null ? null : c.Owner.UserProfile.ProfilePictureUrl,
                    CourtsCount = c.Courts.Count(x => !x.IsDeleted),
                    c.CreatedAt,
                    ActiveSubscription = c.Subscriptions
                        .Where(s => !s.IsDeleted &&
                                    s.StartDate <= DateTime.UtcNow &&
                                    s.EndDate >= DateTime.UtcNow)
                        .Select(s => new
                        {
                            s.Id,
                            s.StartDate,
                            s.EndDate,
                            s.PlanId,
                            PlanName = s.Plan.Name,
                            PlanPrice = s.Plan.MonthlyPrice,
                            PlanMaxCourts = s.Plan.MaxCourts
                        })
                        .FirstOrDefault()
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (club is null)
                return Result.Failure<ClubResponse>(ClubErrors.ClubNotFound);

            var reviewRatings = await _context.Reviews
                .Where(r => r.Court.ClubId == clubId && !r.IsDeleted)
                .Select(r => r.Rating)
                .ToListAsync(ct);

            var isOwner = club.OwnerId == currentUserId;

            var response = new ClubResponse(
                club.Id,
                club.Name,
                club.LogoUrl,
                club.Governorate,
                club.City,
                club.Address,
                club.PhoneNumber,
                club.Email,
                club.IsActive,
                new UserSummary(club.OwnerId, club.OwnerFullName, club.OwnerPicture),
                IsOwner: isOwner,
                CanManageCourts: isOwner,
                CourtsCount: club.CourtsCount,
                ReviewsCount: reviewRatings.Count,
                AverageRating: reviewRatings.Count == 0 ? 0 : Math.Round(reviewRatings.Average(), 1),
                ActiveSubscription: club.ActiveSubscription is null
                    ? null
                    : new ClubSubscriptionSummary(
                        club.ActiveSubscription.Id,
                        new SubscriptionPlanSummary(
                            club.ActiveSubscription.PlanId,
                            club.ActiveSubscription.PlanName,
                            club.ActiveSubscription.PlanPrice,
                            club.ActiveSubscription.PlanMaxCourts),
                        club.ActiveSubscription.StartDate,
                        club.ActiveSubscription.EndDate,
                        IsActive: true),
                club.CreatedAt
            );

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving club {ClubId}", clubId);
            return Result.Failure<ClubResponse>(ClubErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Browse Clubs (public discovery — active only)
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<PaginatedList<ClubResponse>>> GetClubsAsync(
    string? currentUserId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var reviewStats = _context.Reviews
                .Where(r => !r.IsDeleted)
                .GroupBy(r => r.Court.ClubId)
                .Select(g => new
                {
                    ClubId = g.Key,
                    Count = (int?)g.Count(),
                    Average = (double?)g.Average(r => (double)r.Rating)
                });

            var clubsQuery = _context.Clubs
                .Where(c => !c.IsDeleted && c.IsActive)
                .ApplyFilters(filters,
                    searchPredicate: c =>
                        (c.Name != null && c.Name.Contains(filters.SearchValue!)) ||
                        (c.City != null && c.City.Contains(filters.SearchValue!)) ||
                        (c.Governorate != null && c.Governorate.Contains(filters.SearchValue!)),
                    allowedSortColumns: AllowedClubSortColumns);

            var query =
                from c in clubsQuery
                join rs in reviewStats on c.Id equals rs.ClubId into ratingsGroup
                from rs in ratingsGroup.DefaultIfEmpty()
                select new ClubResponse(
                    c.Id,
                    c.Name,
                    c.LogoUrl,
                    c.Governorate,
                    c.City,
                    c.Address,
                    c.PhoneNumber,
                    c.Email,
                    c.IsActive,
                    new UserSummary(
                        c.OwnerId,
                        c.Owner.FullName,
                        c.Owner.UserProfile == null ? null : c.Owner.UserProfile.ProfilePictureUrl),
                    IsOwner: c.OwnerId == currentUserId,
                    CanManageCourts: c.OwnerId == currentUserId,
                    CourtsCount: c.Courts.Count(x => !x.IsDeleted),
                    ReviewsCount: rs.Count ?? 0,
                    AverageRating: rs.Average ?? 0,
                    ActiveSubscription: c.Subscriptions
                        .Where(s => !s.IsDeleted &&
                                    s.StartDate <= DateTime.UtcNow &&
                                    s.EndDate >= DateTime.UtcNow)
                        .Select(s => new ClubSubscriptionSummary(
                            s.Id,
                            new SubscriptionPlanSummary(s.PlanId, s.Plan.Name, s.Plan.MonthlyPrice, s.Plan.MaxCourts),
                            s.StartDate,
                            s.EndDate,
                            true))
                        .FirstOrDefault(),
                    c.CreatedAt
                );

            var result = await query.AsNoTracking().ToPaginatedListAsync(filters, ct);
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving clubs");
            return Result.Failure<PaginatedList<ClubResponse>>(ClubErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Create Club
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<ClubResponse>> CreateClubAsync(
        string ownerId, CreateClubRequest request, CancellationToken ct = default)
    {
        try
        {
            var owner = await _context.Users
                .Where(u => u.Id == ownerId)
                .Select(u => new
                {
                    u.FullName,
                    ProfilePictureUrl = u.UserProfile == null ? null : u.UserProfile.ProfilePictureUrl
                })
                .FirstOrDefaultAsync(ct);

            if (owner is null)
                return Result.Failure<ClubResponse>(UserErrors.UserNotFound);

            var club = new Club
            {
                OwnerId = ownerId,
                Name = request.Name,
                Governorate = request.Governorate,
                City = request.City,
                Address = request.Address,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                IsActive = true
            };

            if (request.Logo is not null)
                club.LogoUrl = await FileHelper.UploadeFileAsync(request.Logo, LogoLocation, _env, _accessor);

            await _context.Clubs.AddAsync(club, ct);
            await _context.SaveChangesAsync(ct);

            var response = new ClubResponse(
                club.Id,
                club.Name,
                club.LogoUrl,
                club.Governorate,
                club.City,
                club.Address,
                club.PhoneNumber,
                club.Email,
                club.IsActive,
                new UserSummary(ownerId, owner.FullName, owner.ProfilePictureUrl),
                IsOwner: true,
                CanManageCourts: true,
                CourtsCount: 0,
                ReviewsCount: 0,
                AverageRating: 0,
                ActiveSubscription: null,
                club.CreatedAt
            );

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating club for owner {OwnerId}", ownerId);
            return Result.Failure<ClubResponse>(ClubErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Update Club
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<ClubResponse>> UpdateClubAsync(
        string userId, string clubId, UpdateClubRequest request, CancellationToken ct = default)
    {
        try
        {
            var club = await _context.Clubs
                .Include(c => c.Owner)
                    .ThenInclude(o => o.UserProfile)
                .FirstOrDefaultAsync(c => c.Id == clubId && !c.IsDeleted, ct);

            if (club is null)
                return Result.Failure<ClubResponse>(ClubErrors.ClubNotFound);

            if (club.OwnerId != userId)
                return Result.Failure<ClubResponse>(ClubErrors.Unauthorized);

            if (request.Name is not null) club.Name = request.Name;
            if (request.Governorate is not null) club.Governorate = request.Governorate;
            if (request.City is not null) club.City = request.City;
            if (request.Address is not null) club.Address = request.Address;
            if (request.PhoneNumber is not null) club.PhoneNumber = request.PhoneNumber;
            if (request.Email is not null) club.Email = request.Email;
            club.IsActive = request.IsActive;

            if (request.Logo is not null)
            {
                var oldLogo = club.LogoUrl;
                club.LogoUrl = await FileHelper.UploadeFileAsync(request.Logo, LogoLocation, _env, _accessor);

                if (!string.IsNullOrEmpty(oldLogo))
                    FileHelper.DeleteFile(oldLogo, LogoLocation, _env);
            }

            await _context.SaveChangesAsync(ct);

            var courtsCount = await _context.Courts
                .CountAsync(x => x.ClubId == clubId && !x.IsDeleted, ct);

            var reviewRatings = await _context.Reviews
                .Where(r => r.Court.ClubId == clubId && !r.IsDeleted)
                .Select(r => r.Rating)
                .ToListAsync(ct);

            var activeSub = await _context.ClubSubscriptions
                .Where(s => s.ClubId == clubId && !s.IsDeleted &&
                            s.StartDate <= DateTime.UtcNow && s.EndDate >= DateTime.UtcNow)
                .Select(s => new ClubSubscriptionSummary(
                    s.Id,
                    new SubscriptionPlanSummary(s.PlanId, s.Plan.Name, s.Plan.MonthlyPrice, s.Plan.MaxCourts),
                    s.StartDate,
                    s.EndDate,
                    true))
                .FirstOrDefaultAsync(ct);

            var response = new ClubResponse(
                club.Id,
                club.Name,
                club.LogoUrl,
                club.Governorate,
                club.City,
                club.Address,
                club.PhoneNumber,
                club.Email,
                club.IsActive,
                new UserSummary(
                    club.OwnerId,
                    club.Owner.FullName,
                    club.Owner.UserProfile == null ? null : club.Owner.UserProfile.ProfilePictureUrl),
                IsOwner: true,
                CanManageCourts: true,
                CourtsCount: courtsCount,
                ReviewsCount: reviewRatings.Count,
                AverageRating: reviewRatings.Count == 0 ? 0 : Math.Round(reviewRatings.Average(), 1),
                ActiveSubscription: activeSub,
                club.CreatedAt
            );

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating club {ClubId} for user {UserId}", clubId, userId);
            return Result.Failure<ClubResponse>(ClubErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Delete Club (soft delete)
    // ════════════════════════════════════════════════════════════════

    public async Task<Result> DeleteClubAsync(
        string userId, string clubId, CancellationToken ct = default)
    {
        try
        {
            var club = await _context.Clubs
                .FirstOrDefaultAsync(c => c.Id == clubId && !c.IsDeleted, ct);

            if (club is null)
                return Result.Failure(ClubErrors.ClubNotFound);

            if (club.OwnerId != userId)
                return Result.Failure(ClubErrors.Unauthorized);

            club.IsDeleted = true;
            club.IsActive = false;

            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting club {ClubId} for user {UserId}", clubId, userId);
            return Result.Failure(ClubErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Toggle Active/Inactive
    // ════════════════════════════════════════════════════════════════

    public async Task<Result> ToggleClubStatusAsync(
        string userId, string clubId, CancellationToken ct = default)
    {
        try
        {
            var club = await _context.Clubs
                .FirstOrDefaultAsync(c => c.Id == clubId && !c.IsDeleted, ct);

            if (club is null)
                return Result.Failure(ClubErrors.ClubNotFound);

            if (club.OwnerId != userId)
                return Result.Failure(ClubErrors.Unauthorized);

            club.IsActive = !club.IsActive;
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while toggling status for club {ClubId} by user {UserId}", clubId, userId);
            return Result.Failure(ClubErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  My Clubs (owner dashboard — includes inactive)
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<PaginatedList<ClubResponse>>> GetMyClubsAsync(
    string userId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var reviewStats = _context.Reviews
                .Where(r => !r.IsDeleted)
                .GroupBy(r => r.Court.ClubId)
                .Select(g => new
                {
                    ClubId = g.Key,
                    Count = (int?)g.Count(),
                    Average = (double?)g.Average(r => (double)r.Rating)
                });

            var clubsQuery = _context.Clubs
                .Where(c => c.OwnerId == userId && !c.IsDeleted)
                .ApplyFilters(filters,
                    searchPredicate: c => c.Name != null && c.Name.Contains(filters.SearchValue!),
                    allowedSortColumns: AllowedClubSortColumns);

            var query =
                from c in clubsQuery
                join rs in reviewStats on c.Id equals rs.ClubId into ratingsGroup
                from rs in ratingsGroup.DefaultIfEmpty()
                select new ClubResponse(
                    c.Id,
                    c.Name,
                    c.LogoUrl,
                    c.Governorate,
                    c.City,
                    c.Address,
                    c.PhoneNumber,
                    c.Email,
                    c.IsActive,
                    new UserSummary(
                        c.OwnerId,
                        c.Owner.FullName,
                        c.Owner.UserProfile == null ? null : c.Owner.UserProfile.ProfilePictureUrl),
                    IsOwner: true,
                    CanManageCourts: true,
                    CourtsCount: c.Courts.Count(x => !x.IsDeleted),
                    ReviewsCount: rs.Count ?? 0,
                    AverageRating: rs.Average ?? 0,
                    ActiveSubscription: c.Subscriptions
                        .Where(s => !s.IsDeleted &&
                                    s.StartDate <= DateTime.UtcNow &&
                                    s.EndDate >= DateTime.UtcNow)
                        .Select(s => new ClubSubscriptionSummary(
                            s.Id,
                            new SubscriptionPlanSummary(s.PlanId, s.Plan.Name, s.Plan.MonthlyPrice, s.Plan.MaxCourts),
                            s.StartDate,
                            s.EndDate,
                            true))
                        .FirstOrDefault(),
                    c.CreatedAt
                );

            var result = await query.AsNoTracking().ToPaginatedListAsync(filters, ct);
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving clubs owned by user {UserId}", userId);
            return Result.Failure<PaginatedList<ClubResponse>>(ClubErrors.Error);
        }
    }
}