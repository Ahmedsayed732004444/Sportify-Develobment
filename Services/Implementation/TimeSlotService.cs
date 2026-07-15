using Microsoft.Data.SqlClient;
using Sportiva.Contracts.Shared.Summaries;
using Sportiva.Contracts.TimeSlots;
namespace Sportiva.Services;

public class TimeSlotService(
    ApplicationDbContext context,
    ILogger<TimeSlotService> logger,
    IOptions<TimeSlotOptions> timeSlotOptions) : ITimeSlotService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<TimeSlotService> _logger = logger;
    private readonly TimeSlotOptions _options = timeSlotOptions.Value;

    // ════════════════════════════════════════════════════════════════
    //  Get Time Slots (public — بيرجع slots يوم معين أو كل الأيام)
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<IReadOnlyList<TimeSlotResponse>>> GetTimeSlotsAsync(
        string courtId, DateOnly? date = null, bool? available = null, CancellationToken ct = default)
    {
        try
        {
            var court = await _context.Courts
                .Include(c => c.Club)
                .FirstOrDefaultAsync(c => c.Id == courtId && !c.IsDeleted, ct);

            if (court is null)
                return Result.Failure<IReadOnlyList<TimeSlotResponse>>(CourtErrors.CourtNotFound);

            var query = _context.TimeSlots
                .Where(ts => ts.CourtId == courtId && !ts.IsDeleted)
                .Where(ts => date == null || ts.Day == date.Value);

            if (available == true)
                query = query.Where(ts => ts.IsActive &&
                    !ts.Bookings.Any(b => !b.IsDeleted &&
                        (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending)));
            else if (available == false)
                query = query.Where(ts => !ts.IsActive ||
                    ts.Bookings.Any(b => !b.IsDeleted &&
                        (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending)));

            var slots = await query
                .OrderBy(ts => ts.Day).ThenBy(ts => ts.StartTime)
                .Select(ts => new
                {
                    ts.Id,
                    ts.Day,
                    ts.StartTime,
                    ts.EndTime,
                    ts.IsActive,
                    ts.CreatedAt,
                    IsBooked = ts.Bookings.Any(b => !b.IsDeleted &&
                        (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending))
                })
                .AsNoTracking()
                .ToListAsync(ct);

            var courtSummary = new CourtSummary(
                court.Id, court.Name, court.ImageUrl,
                (Contracts.Shared.Enums.SportTypeDto)(int)court.SportType, court.PricePerHour,
                new ClubSummary(court.Club.Id, court.Club.Name, court.Club.LogoUrl, court.Club.City, court.Club.Governorate));

            var result = slots.Select(ts => new TimeSlotResponse(
                ts.Id, courtSummary, ts.Day, ts.StartTime, ts.EndTime, ts.IsActive, ts.IsBooked, ts.CreatedAt));

            return Result.Success<IReadOnlyList<TimeSlotResponse>>(result.ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving time slots for court {CourtId}", courtId);
            return Result.Failure<IReadOnlyList<TimeSlotResponse>>(TimeSlotErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Get Single Time Slot
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<TimeSlotResponse>> GetTimeSlotAsync(
        string courtId, string slotId, CancellationToken ct = default)
    {
        try
        {
            var slot = await _context.TimeSlots
                .Include(ts => ts.Court).ThenInclude(c => c.Club)
                .Include(ts => ts.Bookings)
                .FirstOrDefaultAsync(ts => ts.Id == slotId && ts.CourtId == courtId && !ts.IsDeleted, ct);

            if (slot is null)
                return Result.Failure<TimeSlotResponse>(TimeSlotErrors.TimeSlotNotFound);

            var court = slot.Court;
            var courtSummary = new CourtSummary(
                court.Id, court.Name, court.ImageUrl,
                (Contracts.Shared.Enums.SportTypeDto)(int)court.SportType, court.PricePerHour,
                new ClubSummary(court.Club.Id, court.Club.Name, court.Club.LogoUrl, court.Club.City, court.Club.Governorate));

            var response = new TimeSlotResponse(
                slot.Id, courtSummary, slot.Day, slot.StartTime, slot.EndTime,
                slot.IsActive, slot.IsBooked, slot.CreatedAt);

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving time slot {SlotId} for court {CourtId}", slotId, courtId);
            return Result.Failure<TimeSlotResponse>(TimeSlotErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Set Availability (Owner control — bulk activate/deactivate)
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<IReadOnlyList<TimeSlotResponse>>> SetTimeSlotsAvailabilityAsync(
        string userId, string courtId, IReadOnlyList<string> slotIds, bool isActive, CancellationToken ct = default)
    {
        try
        {
            var court = await _context.Courts
                .Include(c => c.Club)
                .FirstOrDefaultAsync(c => c.Id == courtId && !c.IsDeleted, ct);

            if (court is null)
                return Result.Failure<IReadOnlyList<TimeSlotResponse>>(CourtErrors.CourtNotFound);

            if (court.Club.OwnerId != userId)
                return Result.Failure<IReadOnlyList<TimeSlotResponse>>(TimeSlotErrors.Unauthorized);

            var slots = await _context.TimeSlots
                .Where(ts => ts.CourtId == courtId && !ts.IsDeleted && slotIds.Contains(ts.Id))
                .Include(ts => ts.Bookings)
                .ToListAsync(ct);

            if (slots.Count != slotIds.Count)
                return Result.Failure<IReadOnlyList<TimeSlotResponse>>(TimeSlotErrors.SomeSlotsNotFound);

            foreach (var slot in slots)
                slot.IsActive = isActive;

            await _context.SaveChangesAsync(ct);

            var courtSummary = new CourtSummary(
                court.Id, court.Name, court.ImageUrl,
                (Contracts.Shared.Enums.SportTypeDto)(int)court.SportType, court.PricePerHour,
                new ClubSummary(court.Club.Id, court.Club.Name, court.Club.LogoUrl, court.Club.City, court.Club.Governorate));

            var result = slots.Select(ts => new TimeSlotResponse(
                ts.Id, courtSummary, ts.Day, ts.StartTime, ts.EndTime, ts.IsActive, ts.IsBooked, ts.CreatedAt));

            return Result.Success<IReadOnlyList<TimeSlotResponse>>(result.ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while setting availability for time slots on court {CourtId}", courtId);
            return Result.Failure<IReadOnlyList<TimeSlotResponse>>(TimeSlotErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Generate Weekly Time Slots (system-triggered — Hangfire job)
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<int>> GenerateWeeklyTimeSlotsForCourtAsync(
        string courtId, CancellationToken ct = default)
    {
        try
        {
            var court = await _context.Courts
                .FirstOrDefaultAsync(c => c.Id == courtId && !c.IsDeleted, ct);

            if (court is null)
                return Result.Failure<int>(CourtErrors.CourtNotFound);

            if (!court.IsActive)
                return Result.Failure<int>(TimeSlotErrors.CourtNotActive);

            // ✅ بنحدد "اليوم" بتوقيت القاهرة مش UTC مباشرة، عشان قريب من منتصف
            //    الليل بتوقيت مصر الاعتماد على UtcNow ممكن يدّي يوم غلط.
            var today = GetTodayInConfiguredTimeZone();
            var lastDay = today.AddDays(_options.DaysToGenerate - 1);

            // ✅ الـ slots الموجودة بالفعل في الفترة دي عشان نتجنب التكرار (idempotency)
            var existing = await _context.TimeSlots
                .Where(ts => ts.CourtId == courtId && !ts.IsDeleted &&
                             ts.Day >= today && ts.Day <= lastDay)
                .Select(ts => new { ts.Day, ts.StartTime })
                .ToListAsync(ct);

            var existingSet = existing
                .Select(e => (e.Day, e.StartTime))
                .ToHashSet();

            var newSlots = new List<TimeSlot>();

            for (var day = today; day <= lastDay; day = day.AddDays(1))
            {
                for (var hour = _options.OpeningHour; hour < _options.ClosingHour; hour += _options.SlotDurationHours)
                {
                    var startTime = new TimeOnly(hour % 24, 0);
                    var endHour = hour + _options.SlotDurationHours;

                    // ✅ لو الـ Slot الأخير بيقفل الساعة 24 (منتصف الليل)، TimeOnly مش
                    //    بتقدر تمثل 24:00 فبتترجم لـ 00:00 — ده يمثل نفس اللحظة (منتصف
                    //    الليل) مش خطأ، لكنه معناه إن EndTime ممكن يبقى أصغر من
                    //    StartTime لآخر Slot في اليوم. أي كود تاني بيحسب مدة الـ Slot
                    //    (EndTime - StartTime) لازم ياخد الحالة دي في الاعتبار.
                    var endTime = new TimeOnly(endHour % 24, 0);

                    if (existingSet.Contains((day, startTime)))
                        continue; // ✅ الـ slot ده موجود بالفعل — تخطاه

                    newSlots.Add(new TimeSlot
                    {
                        CourtId = courtId,
                        Day = day,
                        StartTime = startTime,
                        EndTime = endTime,
                        IsActive = true
                    });
                }
            }

            if (newSlots.Count == 0)
                return Result.Success(0);

            await _context.TimeSlots.AddRangeAsync(newSlots, ct);

            try
            {
                await _context.SaveChangesAsync(ct);
            }
            catch (DbUpdateException dbEx) when (IsDuplicateKeyViolation(dbEx))
            {
                // ✅ لو الـ job اشتغل مرتين في نفس الوقت لنفس الملعب (race condition)،
                //    الـ unique index في TimeSlotConfiguration هيرفض الصفوف المكررة.
                //    مش بننهار، بس بنسجل تحذير ونرجّع 0 لأن التوليد التاني مكملش.
                _logger.LogWarning(dbEx,
                    "Duplicate time slots detected while generating for court {CourtId} — likely a concurrent job run.",
                    courtId);
                return Result.Success(0);
            }

            return Result.Success(newSlots.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while generating weekly time slots for court {CourtId}", courtId);
            return Result.Failure<int>(TimeSlotErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Helpers
    // ════════════════════════════════════════════════════════════════

    private DateOnly GetTodayInConfiguredTimeZone()
    {
        var timeZone = ResolveTimeZone();
        var nowInZone = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
        return DateOnly.FromDateTime(nowInZone);
    }

    private TimeZoneInfo ResolveTimeZone()
    {
        try
        {
            // IANA ID — بيشتغل على Linux/Docker (البيئة الغالبة للـ hosting).
            return TimeZoneInfo.FindSystemTimeZoneById(_options.TimeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            try
            {
                // Fallback لـ Windows ID لو السيرفر شغال Windows.
                return TimeZoneInfo.FindSystemTimeZoneById(_options.WindowsTimeZoneId);
            }
            catch (TimeZoneNotFoundException ex)
            {
                _logger.LogWarning(ex,
                    "Could not resolve time zone {TimeZoneId} or {WindowsTimeZoneId}, falling back to UTC.",
                    _options.TimeZoneId, _options.WindowsTimeZoneId);
                return TimeZoneInfo.Utc;
            }
        }
    }

    private static bool IsDuplicateKeyViolation(DbUpdateException ex) =>
        ex.InnerException is SqlException sqlEx &&
        (sqlEx.Number == 2601 || sqlEx.Number == 2627);
}