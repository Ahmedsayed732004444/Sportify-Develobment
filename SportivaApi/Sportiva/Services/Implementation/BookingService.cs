using Sportiva.Contracts.Bookings;
using Sportiva.Contracts.Common;
using Sportiva.Contracts.Shared.Enums;
using Sportiva.Contracts.Shared.Summaries;
using Sportiva.Extensions;
using System.Linq.Expressions;

namespace Sportiva.Services;

public class BookingService(
    ApplicationDbContext context,
    INotificationService notificationService,
    ILogger<BookingService> logger) : IBookingService
{
    private readonly ApplicationDbContext _context = context;
    private readonly INotificationService _notificationService = notificationService;
    private readonly ILogger<BookingService> _logger = logger;

    private static readonly string[] AllowedBookingSortColumns = ["BookingDate", "Price"];

    // ════════════════════════════════════════════════════════════════
    //  Projection — بيتحمّل مرة واحدة ويتستخدم في كل الـ queries عشان
    //  منكررش نفس الـ Select في كل method (Court + Club + TimeSlot + User)
    // ════════════════════════════════════════════════════════════════

    private sealed record BookingProjection(
        string Id,
        BookingStatus Status,
        decimal Price,
        DateTime BookingDate,

        string UserId,
        string BookerFullName,
        string? BookerPicture,

        string CourtId,
        string? CourtName,
        string? CourtImageUrl,
        SportType CourtSportType,
        decimal CourtPricePerHour,

        string ClubId,
        string? ClubName,
        string? ClubLogoUrl,
        string? ClubCity,
        string? ClubGovernorate,
        string ClubOwnerId,

        string TimeSlotId,
        DateOnly SlotDay,
        TimeOnly SlotStart,
        TimeOnly SlotEnd
    );

    private static readonly Expression<Func<Booking, BookingProjection>> ToProjection = b => new BookingProjection(
        b.Id, b.Status, b.Price, b.BookingDate,

        b.UserId,
        b.User.FullName,
        b.User.UserProfile == null ? null : b.User.UserProfile.ProfilePictureUrl,

        b.CourtId,
        b.Court.Name,
        b.Court.ImageUrl,
        b.Court.SportType,
        b.Court.PricePerHour,

        b.Court.ClubId,
        b.Court.Club.Name,
        b.Court.Club.LogoUrl,
        b.Court.Club.City,
        b.Court.Club.Governorate,
        b.Court.Club.OwnerId,

        b.TimeSlotId,
        b.TimeSlot.Day,
        b.TimeSlot.StartTime,
        b.TimeSlot.EndTime
    );

    // ════════════════════════════════════════════════════════════════
    //  Get Single Booking
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<BookingResponse>> GetBookingAsync(
        string bookingId, string currentUserId, CancellationToken ct = default)
    {
        try
        {
            var booking = await _context.Bookings
                .Where(b => b.Id == bookingId && !b.IsDeleted)
                .Select(ToProjection)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (booking is null)
                return Result.Failure<BookingResponse>(BookingErrors.BookingNotFound);

            if (booking.UserId != currentUserId && booking.ClubOwnerId != currentUserId)
                return Result.Failure<BookingResponse>(BookingErrors.Unauthorized);

            var existingReview = await GetExistingReviewAsync(bookingId, ct);

            return Result.Success(ToResponse(booking, currentUserId, existingReview));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving booking {BookingId}", bookingId);
            return Result.Failure<BookingResponse>(BookingErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Get My Bookings (Member — كل حجوزاته، بفلتر Status اختياري)
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<PaginatedList<BookingResponse>>> GetMyBookingsAsync(
        string userId, RequestFilters filters, BookingStatus? status = null, CancellationToken ct = default)
    {
        try
        {
            var query = _context.Bookings
                .Where(b => b.UserId == userId && !b.IsDeleted)
                .Where(b => status == null || b.Status == status)
                .ApplyFilters(filters, allowedSortColumns: AllowedBookingSortColumns)
                .Select(ToProjection);

            var paged = await query.AsNoTracking().ToPaginatedListAsync(filters, ct);
            var reviews = await GetExistingReviewsAsync(paged.Items.Select(b => b.Id), ct);

            var result = paged.Select(b => ToResponse(b, userId, reviews.GetValueOrDefault(b.Id)));

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving bookings for user {UserId}", userId);
            return Result.Failure<PaginatedList<BookingResponse>>(BookingErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Get Court Bookings (Owner — إدارة طلبات ملعب واحد بعينه)
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<PaginatedList<BookingResponse>>> GetCourtBookingsAsync(
        string userId, string courtId, RequestFilters filters, DateOnly? date = null, CancellationToken ct = default)
    {
        try
        {
            var court = await _context.Courts
                .Include(c => c.Club)
                .FirstOrDefaultAsync(c => c.Id == courtId && !c.IsDeleted, ct);

            if (court is null)
                return Result.Failure<PaginatedList<BookingResponse>>(CourtErrors.CourtNotFound);

            if (court.Club.OwnerId != userId)
                return Result.Failure<PaginatedList<BookingResponse>>(BookingErrors.Unauthorized);

            var query = _context.Bookings
                .Where(b => b.CourtId == courtId && !b.IsDeleted)
                .Where(b => date == null || b.TimeSlot.Day == date.Value)
                .ApplyFilters(filters, allowedSortColumns: AllowedBookingSortColumns)
                .Select(ToProjection);

            var paged = await query.AsNoTracking().ToPaginatedListAsync(filters, ct);
            var reviews = await GetExistingReviewsAsync(paged.Items.Select(b => b.Id), ct);

            var result = paged.Select(b => ToResponse(b, userId, reviews.GetValueOrDefault(b.Id)));

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving bookings for court {CourtId}", courtId);
            return Result.Failure<PaginatedList<BookingResponse>>(BookingErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Get Club Bookings (Owner — كل الطلبات على مستوى النادي كله)
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<PaginatedList<BookingResponse>>> GetClubBookingsAsync(
        string userId, string clubId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var club = await _context.Clubs
                .FirstOrDefaultAsync(c => c.Id == clubId && !c.IsDeleted, ct);

            if (club is null)
                return Result.Failure<PaginatedList<BookingResponse>>(ClubErrors.ClubNotFound);

            if (club.OwnerId != userId)
                return Result.Failure<PaginatedList<BookingResponse>>(BookingErrors.Unauthorized);

            var query = _context.Bookings
                .Where(b => b.Court.ClubId == clubId && !b.IsDeleted)
                .ApplyFilters(filters, allowedSortColumns: AllowedBookingSortColumns)
                .Select(ToProjection);

            var paged = await query.AsNoTracking().ToPaginatedListAsync(filters, ct);
            var reviews = await GetExistingReviewsAsync(paged.Items.Select(b => b.Id), ct);

            var result = paged.Select(b => ToResponse(b, userId, reviews.GetValueOrDefault(b.Id)));

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving bookings for club {ClubId}", clubId);
            return Result.Failure<PaginatedList<BookingResponse>>(BookingErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Create Booking (Member — بيبعت طلب حجز على Slot فاضي)
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<BookingResponse>> CreateBookingAsync(
        string userId, CreateBookingRequest request, CancellationToken ct = default)
    {
        try
        {
            var court = await _context.Courts
                .Include(c => c.Club)
                .FirstOrDefaultAsync(c => c.Id == request.CourtId && !c.IsDeleted, ct);

            if (court is null)
                return Result.Failure<BookingResponse>(CourtErrors.CourtNotFound);

            if (!court.IsActive)
                return Result.Failure<BookingResponse>(BookingErrors.CourtNotActive);

            var slot = await _context.TimeSlots
                .Include(ts => ts.Bookings)
                .FirstOrDefaultAsync(
                    ts => ts.Id == request.TimeSlotId && ts.CourtId == request.CourtId && !ts.IsDeleted, ct);

            if (slot is null)
                return Result.Failure<BookingResponse>(TimeSlotErrors.TimeSlotNotFound);

            // ✅ IsBooked بتتأكد من Confirmed أو Pending، يعني مينفعش يتبعت طلب
            // تاني على نفس الـ Slot وهو لسه مستني رد صاحب الملعب
            if (!slot.IsActive || slot.IsBooked)
                return Result.Failure<BookingResponse>(BookingErrors.TimeSlotNotAvailable);

            var booking = new Booking
            {
                CourtId = court.Id,
                UserId = userId,
                TimeSlotId = slot.Id,
                Price = court.PricePerHour, // للعرض فقط — مفيش دفع فعلي جوه السيستم
                Status = BookingStatus.Pending
            };

            await _context.Bookings.AddAsync(booking, ct);
            await _context.SaveChangesAsync(ct);

            // Send notification to Court Owner
            await _notificationService.SendNotificationAsync(
                recipientId: court.Club.OwnerId,
                type: Sportiva.Entities.NotificationType.GeneralInfo,
                title: "New Booking Request",
                body: $"A new booking request has been submitted for {court.Name} on {slot.Day:yyyy-MM-dd} at {slot.StartTime:hh:mm tt} - {slot.EndTime:hh:mm tt}.",
                actorId: userId,
                entityType: "Booking",
                entityId: booking.Id,
                priority: Sportiva.Entities.NotificationPriority.Normal,
                ct: ct
            );

            var projected = await _context.Bookings
                .Where(b => b.Id == booking.Id)
                .Select(ToProjection)
                .AsNoTracking()
                .FirstAsync(ct);

            return Result.Success(ToResponse(projected, userId, existingReview: null));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating booking for user {UserId}", userId);
            return Result.Failure<BookingResponse>(BookingErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Review Booking (Owner — يوافق أو يرفض طلب لسه Pending)
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<BookingResponse>> ReviewBookingAsync(
        string ownerId, string bookingId, ReviewBookingRequest request, CancellationToken ct = default)
    {
        try
        {
            var booking = await _context.Bookings
                .Include(b => b.Court).ThenInclude(c => c.Club)
                .Include(b => b.TimeSlot)
                .FirstOrDefaultAsync(b => b.Id == bookingId && !b.IsDeleted, ct);

            if (booking is null)
                return Result.Failure<BookingResponse>(BookingErrors.BookingNotFound);

            if (booking.Court.Club.OwnerId != ownerId)
                return Result.Failure<BookingResponse>(BookingErrors.Unauthorized);

            if (booking.Status != BookingStatus.Pending)
                return Result.Failure<BookingResponse>(BookingErrors.InvalidStatusTransition);

            var newStatus = (BookingStatus)(int)request.NewStatus;

            // ✅ صاحب الملعب حر يوافق أو يرفض بس — أي قيمة تانية (Completed مثلًا) مرفوضة
            if (newStatus != BookingStatus.Confirmed && newStatus != BookingStatus.Rejected)
                return Result.Failure<BookingResponse>(BookingErrors.InvalidStatusTransition);

            booking.Status = newStatus;
            await _context.SaveChangesAsync(ct);

            var actionText = booking.Status == BookingStatus.Confirmed ? "accepted" : "rejected";
            var notificationType = booking.Status == BookingStatus.Confirmed
                ? Sportiva.Entities.NotificationType.BookingConfirmed
                : Sportiva.Entities.NotificationType.BookingCancelled;

            await _notificationService.SendNotificationAsync(
                recipientId: booking.UserId,
                type: notificationType,
                title: $"Booking Request {booking.Status}",
                body: $"Your booking request for {booking.Court.Name} on {booking.TimeSlot.Day:yyyy-MM-dd} has been {actionText}.",
                actorId: ownerId,
                entityType: "Booking",
                entityId: booking.Id,
                priority: Sportiva.Entities.NotificationPriority.Normal,
                ct: ct
            );

            var projected = await _context.Bookings
                .Where(b => b.Id == bookingId)
                .Select(ToProjection)
                .AsNoTracking()
                .FirstAsync(ct);

            return Result.Success(ToResponse(projected, ownerId, existingReview: null));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while reviewing booking {BookingId}", bookingId);
            return Result.Failure<BookingResponse>(BookingErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Cancel Booking
    //  - العضو: يقدر يلغي وهو لسه Pending (سحب الطلب) أو بعد ما اتأكد.
    //  - صاحب الملعب: يقدر يلغي بس لو الحجز Confirmed (Pending بيترفض
    //    مش بيتلغي، عن طريق ReviewBookingAsync).
    // ════════════════════════════════════════════════════════════════

    public async Task<Result> CancelBookingAsync(
        string userId, string bookingId, CancellationToken ct = default)
    {
        try
        {
            var booking = await _context.Bookings
                .Include(b => b.Court).ThenInclude(c => c.Club)
                .Include(b => b.TimeSlot)
                .FirstOrDefaultAsync(b => b.Id == bookingId && !b.IsDeleted, ct);

            if (booking is null)
                return Result.Failure(BookingErrors.BookingNotFound);

            var isMine = booking.UserId == userId;
            var isOwner = booking.Court.Club.OwnerId == userId;

            if (!isMine && !isOwner)
                return Result.Failure(BookingErrors.Unauthorized);

            var canCancel =
                (isMine && (booking.Status == BookingStatus.Pending || booking.Status == BookingStatus.Confirmed)) ||
                (isOwner && booking.Status == BookingStatus.Confirmed);

            if (!canCancel)
                return Result.Failure(BookingErrors.InvalidStatusTransition);

            booking.Status = BookingStatus.Cancelled;
            await _context.SaveChangesAsync(ct);

            var recipientId = isMine ? booking.Court.Club.OwnerId : booking.UserId;
            var cancellerName = isMine ? "The player" : "The club organizer";

            await _notificationService.SendNotificationAsync(
                recipientId: recipientId,
                type: Sportiva.Entities.NotificationType.BookingCancelled,
                title: "Booking Cancelled",
                body: $"{cancellerName} cancelled the booking for {booking.Court.Name} on {booking.TimeSlot.Day:yyyy-MM-dd}.",
                actorId: userId,
                entityType: "Booking",
                entityId: booking.Id,
                priority: Sportiva.Entities.NotificationPriority.Normal,
                ct: ct
            );

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while cancelling booking {BookingId}", bookingId);
            return Result.Failure(BookingErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Get Booking Receipt (Member فقط — الإيصال بتاعه هو)
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<BookingResponse>> GetBookingReceiptAsync(
        string userId, string bookingId, CancellationToken ct = default)
    {
        try
        {
            var booking = await _context.Bookings
                .Where(b => b.Id == bookingId && !b.IsDeleted)
                .Select(ToProjection)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (booking is null)
                return Result.Failure<BookingResponse>(BookingErrors.BookingNotFound);

            if (booking.UserId != userId)
                return Result.Failure<BookingResponse>(BookingErrors.Unauthorized);

            var existingReview = await GetExistingReviewAsync(bookingId, ct);

            return Result.Success(ToResponse(booking, userId, existingReview));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving receipt for booking {BookingId}", bookingId);
            return Result.Failure<BookingResponse>(BookingErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Helpers
    // ════════════════════════════════════════════════════════════════

    private async Task<ReviewSummary?> GetExistingReviewAsync(string bookingId, CancellationToken ct) =>
        await _context.Reviews
            .Where(r => r.BookingId == bookingId && !r.IsDeleted)
            .Select(r => new ReviewSummary(r.Id, r.Rating, r.Comment, r.CreatedAt))
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

    private async Task<Dictionary<string, ReviewSummary>> GetExistingReviewsAsync(
        IEnumerable<string> bookingIds, CancellationToken ct)
    {
        var ids = bookingIds.ToList();

        if (ids.Count == 0)
            return [];

        return await _context.Reviews
            .Where(r => ids.Contains(r.BookingId) && !r.IsDeleted)
            .Select(r => new { r.BookingId, Summary = new ReviewSummary(r.Id, r.Rating, r.Comment, r.CreatedAt) })
            .AsNoTracking()
            .ToDictionaryAsync(x => x.BookingId, x => x.Summary, ct);
    }

    private static BookingResponse ToResponse(
        BookingProjection b, string currentUserId, ReviewSummary? existingReview)
    {
        var isMine = b.UserId == currentUserId;
        var isOwner = b.ClubOwnerId == currentUserId;

        var canCancel =
            (isMine && (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed)) ||
            (isOwner && b.Status == BookingStatus.Confirmed);

        var canRespondToRequest = isOwner && b.Status == BookingStatus.Pending;
        var canReview = isMine && b.Status == BookingStatus.Completed && existingReview is null;

        return new BookingResponse(
            BookingId: b.Id,
            BookingNumber: BuildBookingNumber(b.Id), // ⚠️ مفيش عمود BookingNumber في الـ Entity — ده رقم مُشتق من الـ Id للعرض بس
            Status: (BookingStatusDto)(int)b.Status,
            Price: b.Price,

            Court: new CourtSummary(
                b.CourtId, b.CourtName, b.CourtImageUrl,
                (SportTypeDto)(int)b.CourtSportType, b.CourtPricePerHour,
                new ClubSummary(b.ClubId, b.ClubName, b.ClubLogoUrl, b.ClubCity, b.ClubGovernorate)),

            // ✅ IsBooked هنا بتعكس حالة الحجز ده نفسه (مش استعلام تاني عن السلوت):
            // لو الحجز Pending أو Confirmed يبقى هو ده اللي شاغل السلوت دلوقتي،
            // لو Rejected/Cancelled/Completed يبقى السلوت (منطقيًا) فاضي تاني.
            TimeSlot: new TimeSlotSummary(
                b.TimeSlotId, b.SlotDay, b.SlotStart, b.SlotEnd,
                IsBooked: b.Status is BookingStatus.Pending or BookingStatus.Confirmed),

            BookedBy: new UserSummary(b.UserId, b.BookerFullName, b.BookerPicture),

            IsMine: isMine,
            CanCancel: canCancel,
            CanRespondToRequest: canRespondToRequest,
            CanReview: canReview,

            ExistingReview: existingReview,

            CreatedAt: b.BookingDate
        );
    }

    private static string BuildBookingNumber(string bookingId) =>
        $"BK-{bookingId[^8..].ToUpperInvariant()}";
}