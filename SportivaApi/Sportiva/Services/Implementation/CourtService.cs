using Hangfire;
using Sportiva.Contracts.Common;
using Sportiva.Contracts.Courts;
using Sportiva.Contracts.Shared.Enums;
using Sportiva.Contracts.Shared.Summaries;
using Sportiva.Extensions;

namespace Sportiva.Services;

public class CourtService(
    ApplicationDbContext context,
    ILogger<CourtService> logger,
    IWebHostEnvironment env,
    IHttpContextAccessor accessor,
     ITimeSlotService timeSlotService) : ICourtService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<CourtService> _logger = logger;
    private readonly IWebHostEnvironment _env = env;
    private readonly IHttpContextAccessor _accessor = accessor;

    private static readonly string[] AllowedCourtSortColumns = ["Name", "PricePerHour", "CreatedAt"];
    private const string ImageLocation = "uploads/courts";
    private readonly ITimeSlotService _timeSlotService = timeSlotService;
    // ════════════════════════════════════════════════════════════════
    //  Search Courts (public discovery — active courts only)
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<PaginatedList<CourtResponse>>> SearchCourtsAsync(
        string? currentUserId, RequestFilters filters,
        SportType? sport = null, string? city = null, DateOnly? date = null,
        CancellationToken ct = default)
    {
        try
        {
            var reviewStats = _context.Reviews
                .Where(r => !r.IsDeleted)
                .GroupBy(r => r.CourtId)
                .Select(g => new
                {
                    CourtId = g.Key,
                    Count = (int?)g.Count(),
                    Average = (double?)g.Average(r => (double)r.Rating)
                });

            var courtsQuery = _context.Courts
                .Where(c => !c.IsDeleted && c.IsActive)
                .Where(c => sport == null || c.SportType == sport)
                .Where(c => city == null || (c.Club.City != null && c.Club.City.Contains(city)))
                .Where(c => date == null || c.TimeSlots.Any(ts =>
                    ts.Day == date.Value && !ts.IsDeleted &&
                    !ts.Bookings.Any(b => !b.IsDeleted &&
                        (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending))))
                .ApplyFilters(filters,
                    searchPredicate: c => c.Name != null && c.Name.Contains(filters.SearchValue!),
                    allowedSortColumns: AllowedCourtSortColumns);

            // ✅ بنجيب c.SportType خام من غير Cast لـ int عشان EF يترجم الـ SQL صح
            // (العمود متخزن كـ nvarchar بسبب HasConversion<string> في CourtConfiguration)
            var query =
                from c in courtsQuery
                join rs in reviewStats on c.Id equals rs.CourtId into ratingsGroup
                from rs in ratingsGroup.DefaultIfEmpty()
                select new
                {
                    c.Id,
                    c.Name,
                    c.Description,
                    c.ImageUrl,
                    c.SportType,
                    c.MaxCapacity,
                    c.PricePerHour,
                    c.IsActive,
                    c.ClubId,
                    ClubName = c.Club.Name,
                    ClubLogoUrl = c.Club.LogoUrl,
                    ClubCity = c.Club.City,
                    ClubGovernorate = c.Club.Governorate,
                    ClubOwnerId = c.Club.OwnerId,
                    ReviewsCount = rs.Count ?? 0,
                    AverageRating = rs.Average ?? 0,
                    c.CreatedAt
                };

            var paged = await query.AsNoTracking().ToPaginatedListAsync(filters, ct);

            // ✅ الـ mapping النهائي والـ Enum Cast بيحصلوا هنا في الميموري بعد جلب الداتا
            var result = paged.Select(c => new CourtResponse(
                c.Id,
                c.Name,
                c.Description,
                c.ImageUrl,
                (SportTypeDto)(int)c.SportType,
                c.MaxCapacity,
                c.PricePerHour,
                c.IsActive,
                new ClubSummary(c.ClubId, c.ClubName, c.ClubLogoUrl, c.ClubCity, c.ClubGovernorate),
                CanBook: c.IsActive,
                CanManage: c.ClubOwnerId == currentUserId,
                ReviewsCount: c.ReviewsCount,
                AverageRating: c.AverageRating,
                c.CreatedAt
            ));

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching courts");
            return Result.Failure<PaginatedList<CourtResponse>>(CourtErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Get Courts For a Club
    //  (لو الطالب هو صاحب النادي بيشوف كل الملاعب حتى المتوقفة،
    //   غير كده بيشوف الملاعب النشطة بس)
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<PaginatedList<CourtResponse>>> GetClubCourtsAsync(
        string clubId, string? currentUserId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var club = await _context.Clubs
                .FirstOrDefaultAsync(c => c.Id == clubId && !c.IsDeleted, ct);

            if (club is null)
                return Result.Failure<PaginatedList<CourtResponse>>(ClubErrors.ClubNotFound);

            var isOwner = club.OwnerId == currentUserId;

            var reviewStats = _context.Reviews
                .Where(r => !r.IsDeleted)
                .GroupBy(r => r.CourtId)
                .Select(g => new
                {
                    CourtId = g.Key,
                    Count = (int?)g.Count(),
                    Average = (double?)g.Average(r => (double)r.Rating)
                });

            var courtsQuery = _context.Courts
                .Where(c => c.ClubId == clubId && !c.IsDeleted)
                .Where(c => isOwner || c.IsActive)
                .ApplyFilters(filters,
                    searchPredicate: c => c.Name != null && c.Name.Contains(filters.SearchValue!),
                    allowedSortColumns: AllowedCourtSortColumns);

            var query =
                from c in courtsQuery
                join rs in reviewStats on c.Id equals rs.CourtId into ratingsGroup
                from rs in ratingsGroup.DefaultIfEmpty()
                select new
                {
                    c.Id,
                    c.Name,
                    c.Description,
                    c.ImageUrl,
                    c.SportType,
                    c.MaxCapacity,
                    c.PricePerHour,
                    c.IsActive,
                    ReviewsCount = rs.Count ?? 0,
                    AverageRating = rs.Average ?? 0,
                    c.CreatedAt
                };

            var paged = await query.AsNoTracking().ToPaginatedListAsync(filters, ct);

            var result = paged.Select(c => new CourtResponse(
                c.Id,
                c.Name,
                c.Description,
                c.ImageUrl,
                (SportTypeDto)(int)c.SportType,
                c.MaxCapacity,
                c.PricePerHour,
                c.IsActive,
                new ClubSummary(club.Id, club.Name, club.LogoUrl, club.City, club.Governorate),
                CanBook: c.IsActive,
                CanManage: isOwner,
                ReviewsCount: c.ReviewsCount,
                AverageRating: c.AverageRating,
                c.CreatedAt
            ));

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving courts for club {ClubId}", clubId);
            return Result.Failure<PaginatedList<CourtResponse>>(CourtErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Get Single Court
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<CourtResponse>> GetCourtAsync(
        string clubId, string courtId, string? currentUserId = null, CancellationToken ct = default)
    {
        try
        {
            var court = await _context.Courts
                .Where(c => c.Id == courtId && c.ClubId == clubId && !c.IsDeleted)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Description,
                    c.ImageUrl,
                    c.SportType,
                    c.MaxCapacity,
                    c.PricePerHour,
                    c.IsActive,
                    c.CreatedAt,
                    Club = new ClubSummary(c.ClubId, c.Club.Name, c.Club.LogoUrl, c.Club.City, c.Club.Governorate),
                    OwnerId = c.Club.OwnerId
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (court is null)
                return Result.Failure<CourtResponse>(CourtErrors.CourtNotFound);

            var reviewRatings = await _context.Reviews
                .Where(r => r.CourtId == courtId && !r.IsDeleted)
                .Select(r => r.Rating)
                .ToListAsync(ct);

            var response = new CourtResponse(
                court.Id,
                court.Name,
                court.Description,
                court.ImageUrl,
                (SportTypeDto)(int)court.SportType,
                court.MaxCapacity,
                court.PricePerHour,
                court.IsActive,
                court.Club,
                CanBook: court.IsActive,
                CanManage: court.OwnerId == currentUserId,
                ReviewsCount: reviewRatings.Count,
                AverageRating: reviewRatings.Count == 0 ? 0 : Math.Round(reviewRatings.Average(), 1),
                court.CreatedAt
            );

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving court {CourtId}", courtId);
            return Result.Failure<CourtResponse>(CourtErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Create Court
    //  ⚠️ هنا بيتحقق إن النادي عنده اشتراك فعّال، وإن عدد الملاعب
    //  الحالية (!IsDeleted) لسه تحت حد الـ MaxCourts بتاع خطته.
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<CourtResponse>> CreateCourtAsync(
        string userId, string clubId, CreateCourtRequest request, CancellationToken ct = default)
    {
        try
        {
            var club = await _context.Clubs
                .FirstOrDefaultAsync(c => c.Id == clubId && !c.IsDeleted, ct);

            if (club is null)
                return Result.Failure<CourtResponse>(ClubErrors.ClubNotFound);

            if (club.OwnerId != userId)
                return Result.Failure<CourtResponse>(ClubErrors.Unauthorized);

            var now = DateTime.UtcNow;

            var activeSubscription = await _context.ClubSubscriptions
                .Where(s => s.ClubId == clubId && !s.IsDeleted &&
                            s.StartDate <= now && s.EndDate >= now)
                .Select(s => new { s.Plan.MaxCourts })
                .FirstOrDefaultAsync(ct);

            if (activeSubscription is null)
                return Result.Failure<CourtResponse>(CourtErrors.NoActiveSubscription);

            var currentCourtsCount = await _context.Courts
                .CountAsync(c => c.ClubId == clubId && !c.IsDeleted, ct);

            if (currentCourtsCount >= activeSubscription.MaxCourts)
                return Result.Failure<CourtResponse>(CourtErrors.MaxCourtsReached(activeSubscription.MaxCourts));

            var court = new Court
            {
                ClubId = clubId,
                Name = request.Name,
                Description = request.Description,
                SportType = (SportType)(int)request.SportType,
                MaxCapacity = request.MaxCapacity,
                PricePerHour = request.PricePerHour,
                IsActive = true
            };

            if (request.Image is not null)
                court.ImageUrl = await FileHelper.UploadeFileAsync(request.Image, ImageLocation, _env, _accessor);

            await _context.Courts.AddAsync(court, ct);
            await _context.SaveChangesAsync(ct);

            BackgroundJob.Enqueue<ITimeSlotService>(s => s.GenerateWeeklyTimeSlotsForCourtAsync(court.Id, CancellationToken.None));
            // ✅ توليد أسبوع كامل من الـ TimeSlots مباشرة (مش Background) للتأكد إنها شغالة
            //  var timeSlotsResult = await _timeSlotService.GenerateWeeklyTimeSlotsForCourtAsync(court.Id, ct);

            //if (timeSlotsResult.IsFailure)
            //    _logger.LogWarning(
            //        "Failed to generate initial time slots for court {CourtId}: {Error}",
            //        court.Id, timeSlotsResult.Errors.First().Description);
            var response = new CourtResponse(
                court.Id,
                court.Name,
                court.Description,
                court.ImageUrl,
                (SportTypeDto)(int)court.SportType,
                court.MaxCapacity,
                court.PricePerHour,
                court.IsActive,
                new ClubSummary(club.Id, club.Name, club.LogoUrl, club.City, club.Governorate),
                CanBook: true,
                CanManage: true,
                ReviewsCount: 0,
                AverageRating: 0,
                court.CreatedAt
            );

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating court for club {ClubId}", clubId);
            return Result.Failure<CourtResponse>(CourtErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Update Court
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<CourtResponse>> UpdateCourtAsync(
        string userId, string clubId, string courtId, UpdateCourtRequest request, CancellationToken ct = default)
    {
        try
        {
            var club = await _context.Clubs
                .FirstOrDefaultAsync(c => c.Id == clubId && !c.IsDeleted, ct);

            if (club is null)
                return Result.Failure<CourtResponse>(ClubErrors.ClubNotFound);

            if (club.OwnerId != userId)
                return Result.Failure<CourtResponse>(ClubErrors.Unauthorized);

            var court = await _context.Courts
                .FirstOrDefaultAsync(c => c.Id == courtId && c.ClubId == clubId && !c.IsDeleted, ct);

            if (court is null)
                return Result.Failure<CourtResponse>(CourtErrors.CourtNotFound);

            if (request.Name is not null) court.Name = request.Name;
            if (request.Description is not null) court.Description = request.Description;
            court.SportType = (SportType)(int)request.SportType;
            court.MaxCapacity = request.MaxCapacity;
            court.PricePerHour = request.PricePerHour;
            court.IsActive = request.IsActive;

            if (request.Image is not null)
            {
                var oldImage = court.ImageUrl;
                court.ImageUrl = await FileHelper.UploadeFileAsync(request.Image, ImageLocation, _env, _accessor);

                if (!string.IsNullOrEmpty(oldImage))
                    FileHelper.DeleteFile(oldImage, ImageLocation, _env);
            }

            await _context.SaveChangesAsync(ct);

            var reviewRatings = await _context.Reviews
                .Where(r => r.CourtId == courtId && !r.IsDeleted)
                .Select(r => r.Rating)
                .ToListAsync(ct);

            var response = new CourtResponse(
                court.Id,
                court.Name,
                court.Description,
                court.ImageUrl,
                (SportTypeDto)(int)court.SportType,
                court.MaxCapacity,
                court.PricePerHour,
                court.IsActive,
                new ClubSummary(club.Id, club.Name, club.LogoUrl, club.City, club.Governorate),
                CanBook: court.IsActive,
                CanManage: true,
                ReviewsCount: reviewRatings.Count,
                AverageRating: reviewRatings.Count == 0 ? 0 : Math.Round(reviewRatings.Average(), 1),
                court.CreatedAt
            );

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating court {CourtId} for club {ClubId}", courtId, clubId);
            return Result.Failure<CourtResponse>(CourtErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Delete Court (soft delete)
    // ════════════════════════════════════════════════════════════════

    public async Task<Result> DeleteCourtAsync(
        string userId, string clubId, string courtId, CancellationToken ct = default)
    {
        try
        {
            var club = await _context.Clubs
                .FirstOrDefaultAsync(c => c.Id == clubId && !c.IsDeleted, ct);

            if (club is null)
                return Result.Failure(ClubErrors.ClubNotFound);

            if (club.OwnerId != userId)
                return Result.Failure(ClubErrors.Unauthorized);

            var court = await _context.Courts
                .FirstOrDefaultAsync(c => c.Id == courtId && c.ClubId == clubId && !c.IsDeleted, ct);

            if (court is null)
                return Result.Failure(CourtErrors.CourtNotFound);

            court.IsDeleted = true;
            court.IsActive = false;

            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting court {CourtId} for club {ClubId}", courtId, clubId);
            return Result.Failure(CourtErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Toggle Active/Inactive
    // ════════════════════════════════════════════════════════════════

    public async Task<Result> ToggleCourtStatusAsync(
        string userId, string clubId, string courtId, CancellationToken ct = default)
    {
        try
        {
            var club = await _context.Clubs
                .FirstOrDefaultAsync(c => c.Id == clubId && !c.IsDeleted, ct);

            if (club is null)
                return Result.Failure(ClubErrors.ClubNotFound);

            if (club.OwnerId != userId)
                return Result.Failure(ClubErrors.Unauthorized);

            var court = await _context.Courts
                .FirstOrDefaultAsync(c => c.Id == courtId && c.ClubId == clubId && !c.IsDeleted, ct);

            if (court is null)
                return Result.Failure(CourtErrors.CourtNotFound);

            court.IsActive = !court.IsActive;
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while toggling status for court {CourtId} by user {UserId}", courtId, userId);
            return Result.Failure(CourtErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Court Availability (public — بيرجع الـ TimeSlots بتاعة يوم معين)
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<IReadOnlyList<TimeSlotSummary>>> GetCourtAvailabilityAsync(
        string courtId, DateOnly date, CancellationToken ct = default)
    {
        try
        {
            var courtExists = await _context.Courts
                .AnyAsync(c => c.Id == courtId && !c.IsDeleted, ct);

            if (!courtExists)
                return Result.Failure<IReadOnlyList<TimeSlotSummary>>(CourtErrors.CourtNotFound);

            var slots = await _context.TimeSlots
                .Where(ts => ts.CourtId == courtId && !ts.IsDeleted && ts.Day == date)
                .OrderBy(ts => ts.StartTime)
                .Select(ts => new TimeSlotSummary(
                    ts.Id,
                    ts.Day,
                    ts.StartTime,
                    ts.EndTime,
                    ts.Bookings.Any(b => !b.IsDeleted &&
                        (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending))
                ))
                .AsNoTracking()
                .ToListAsync(ct);

            return Result.Success<IReadOnlyList<TimeSlotSummary>>(slots);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving availability for court {CourtId}", courtId);
            return Result.Failure<IReadOnlyList<TimeSlotSummary>>(CourtErrors.Error);
        }
    }
}