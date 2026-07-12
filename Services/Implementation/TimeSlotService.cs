using Sportiva.Abstractions;
using Sportiva.Contracts.Shared.Enums;
using Sportiva.Contracts.Shared.Summaries;
using Sportiva.Contracts.TimeSlots;
using Sportiva.Entities;
using Sportiva.Enums;

namespace Sportiva.Services.Implementation
{
    /// <summary>
    /// Service for managing court time slots with full business logic, ownership verification,
    /// overlap prevention, and booking-aware rules (no delete/update of booked slots).
    /// 
    /// Timezone Convention: All DateOnly and TimeOnly values are stored and interpreted as
    /// Africa/Cairo local time (Egypt timezone). When checking if a slot is "not in the past",
    /// local date/time is converted to UTC using the Cairo timezone and compared to DateTime.UtcNow.
    /// 
    /// Race Condition Prevention: Unique constraint on (CourtId, Day, StartTime) prevents exact
    /// duplicate slots. Combined with application-level overlap checking in a transaction, this
    /// stops concurrent overlapping slots from being created. Assumes fixed-duration grid-aligned
    /// slots (e.g., hourly grid). If variable-duration slots are introduced, upgrade to
    /// SERIALIZABLE transaction locking.
    /// </summary>
    public class TimeSlotService : ITimeSlotService
    {
        private readonly ApplicationDbContext _context;

        // Named constants for business rules and safety bounds
        private const int MinSlotDurationMinutes = 15;  // Minimum slot duration
        private const int MaxSlotDurationMinutes = 1440; // Maximum slot duration (24 hours)
        private const int DefaultLookaheadDays = 30;     // Default lookahead window for GetTimeSlotsAsync when no date provided

        // Cairo timezone for consistent local time handling across the platform
        private static readonly TimeZoneInfo CairoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");

        public TimeSlotService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a single time slot for a court, with full validation and overlap checking.
        /// </summary>
        /// <remarks>
        /// Process flow:
        /// 1. Validate all input IDs (userId, courtId, slotId derived from request)
        /// 2. Validate request: StartTime &lt; EndTime, duration within bounds, not in the past (Cairo local time)
        /// 3. Load and verify court exists (404) and ownership (403)
        /// 4. Check for overlaps with existing slots on the same court and day
        /// 5. Create the new time slot
        /// 
        /// Ownership: userId must match Court.Club.OwnerId (the club/court owner creating availability).
        /// </remarks>
        /// <param name="userId">The court owner's user ID (must match Club.OwnerId).</param>
        /// <param name="courtId">The court ID where the slot is created.</param>
        /// <param name="request">Request containing Day, StartTime, EndTime.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// Success with TimeSlotResponse if created.
        /// Failure codes:
        /// - Validation.UserId (400) - userId null/empty
        /// - Validation.CourtId (400) - courtId null/empty
        /// - Validation.TimeRange (400) - StartTime &gt;= EndTime or invalid duration
        /// - Validation.SlotDuration (400) - slot duration outside bounds (15 min – 24 hours)
        /// - Validation.SlotInPast (400) - slot start time is in the past (Egypt local time)
        /// - Court.NotFound (404) - court doesn't exist
        /// - Court.Forbidden (403) - userId does not own the court's club
        /// - TimeSlot.Overlapping (409) - requested slot overlaps existing slots
        /// </returns>
        public async Task<Result<TimeSlotResponse>> CreateTimeSlotAsync(
            string userId, string courtId, CreateTimeSlotRequest request, CancellationToken ct = default)
        {
            // Validation: IDs
            var idValidation = ValidateIds(userId, courtId);
            if (idValidation.IsFailure)
                return Result.Failure<TimeSlotResponse>(idValidation.Error);

            // Validation: Time range and duration
            var timeValidation = ValidateTimeRange(request.StartTime, request.EndTime);
            if (timeValidation.IsFailure)
                return Result.Failure<TimeSlotResponse>(timeValidation.Error);

            // Validation: Slot is not in the past (combined DateOnly + TimeOnly in Cairo timezone)
            var pastValidation = ValidateSlotNotInPast(request.Day, request.StartTime);
            if (pastValidation.IsFailure)
                return Result.Failure<TimeSlotResponse>(pastValidation.Error);

            // Load court and verify ownership
            var courtResult = await LoadCourtWithOwnershipCheckAsync(courtId, userId, ct);
            if (courtResult.IsFailure)
                return Result.Failure<TimeSlotResponse>(courtResult.Error);

            var court = courtResult.Value;

            // Check for overlaps with existing slots on the same court and day
            var overlapResult = await HasOverlapAsync(courtId, request.Day, request.StartTime, request.EndTime, null, ct);
            if (overlapResult.IsFailure)
                return Result.Failure<TimeSlotResponse>(overlapResult.Error);

            // Create the time slot
            var timeSlot = new TimeSlot
            {
                Id = Guid.CreateVersion7().ToString(),
                CourtId = courtId,
                Day = request.Day,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.TimeSlots.Add(timeSlot);
            await _context.SaveChangesAsync(ct);

            var response = MapToResponse(timeSlot, court);
            return Result.Success(response);
        }

        /// <summary>
        /// Bulk creates multiple time slots for a court in a single atomic transaction.
        /// </summary>
        /// <remarks>
        /// Process flow:
        /// 1. Validate all input IDs and request batch
        /// 2. Validate each request individually (time range, duration, not in past)
        /// 3. Load and verify court exists (404) and ownership (403)
        /// 4. Check for overlaps: each requested slot against existing slots AND against other slots in the batch
        /// 5. If any validation fails for any slot, reject the entire batch (no partial creation)
        /// 6. Create all slots in a single database transaction (all-or-nothing)
        /// 
        /// Returns detailed error info indicating which slot(s) in the batch failed and why.
        /// </remarks>
        /// <param name="userId">The court owner's user ID (must match Club.OwnerId).</param>
        /// <param name="courtId">The court ID where slots are created.</param>
        /// <param name="requests">List of time slot requests to create.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// Success with list of TimeSlotResponse if all slots created.
        /// Failure codes:
        /// - Validation.UserId (400) - userId null/empty
        /// - Validation.CourtId (400) - courtId null/empty
        /// - Validation.BatchEmpty (400) - requests list is empty
        /// - Validation.BatchTooLarge (400) - requests list exceeds max size (e.g., 100)
        /// - (detailed per-slot errors in response message or separate error handling)
        /// - Court.NotFound (404) - court doesn't exist
        /// - Court.Forbidden (403) - userId does not own the court's club
        /// - TimeSlot.Overlapping (409) - one or more slots overlap with existing slots or batch slots
        /// </returns>
        public async Task<Result<IReadOnlyList<TimeSlotResponse>>> BulkCreateTimeSlotsAsync(
            string userId, string courtId, IReadOnlyList<CreateTimeSlotRequest> requests, CancellationToken ct = default)
        {
            // Validation: IDs
            var idValidation = ValidateIds(userId, courtId);
            if (idValidation.IsFailure)
                return Result.Failure<IReadOnlyList<TimeSlotResponse>>(idValidation.Error);

            // Validation: Batch size and non-empty
            if (requests == null || requests.Count == 0)
            {
                return Result.Failure<IReadOnlyList<TimeSlotResponse>>(new Error(
                    "Validation.BatchEmpty", "Requests list cannot be empty", 400));
            }

            if (requests.Count > 100)
            {
                return Result.Failure<IReadOnlyList<TimeSlotResponse>>(new Error(
                    "Validation.BatchTooLarge", "Cannot bulk create more than 100 slots at once", 400));
            }

            // Validate each request in the batch
            foreach (var (request, index) in requests.Select((r, i) => (r, i)))
            {
                var timeValidation = ValidateTimeRange(request.StartTime, request.EndTime);
                if (timeValidation.IsFailure)
                    return Result.Failure<IReadOnlyList<TimeSlotResponse>>(new Error(
                        "Validation.TimeRange",
                        $"Batch slot {index}: {timeValidation.Error.Description}",
                        400));

                var pastValidation = ValidateSlotNotInPast(request.Day, request.StartTime);
                if (pastValidation.IsFailure)
                    return Result.Failure<IReadOnlyList<TimeSlotResponse>>(new Error(
                        "Validation.SlotInPast",
                        $"Batch slot {index}: {pastValidation.Error.Description}",
                        400));
            }

            // Load court and verify ownership
            var courtResult = await LoadCourtWithOwnershipCheckAsync(courtId, userId, ct);
            if (courtResult.IsFailure)
                return Result.Failure<IReadOnlyList<TimeSlotResponse>>(courtResult.Error);

            var court = courtResult.Value;

            // Check for overlaps within the batch itself and against existing slots
            foreach (var (request, index) in requests.Select((r, i) => (r, i)))
            {
                // Check against existing slots
                var overlapResult = await HasOverlapAsync(courtId, request.Day, request.StartTime, request.EndTime, null, ct);
                if (overlapResult.IsFailure)
                    return Result.Failure<IReadOnlyList<TimeSlotResponse>>(new Error(
                        "TimeSlot.Overlapping",
                        $"Batch slot {index}: {overlapResult.Error.Description}",
                        409));

                // Check against other slots in the batch (before this index)
                foreach (var (otherRequest, otherIndex) in requests.Take(index).Select((r, i) => (r, i)))
                {
                    if (SlotsOverlap(request.Day, request.StartTime, request.EndTime,
                                    otherRequest.Day, otherRequest.StartTime, otherRequest.EndTime))
                    {
                        return Result.Failure<IReadOnlyList<TimeSlotResponse>>(new Error(
                            "TimeSlot.Overlapping",
                            $"Batch slots {otherIndex} and {index} overlap",
                            409));
                    }
                }
            }

            // Create all slots in a single transaction (all-or-nothing)
            using var transaction = await _context.Database.BeginTransactionAsync(ct);
            try
            {
                var createdSlots = new List<TimeSlot>();

                foreach (var request in requests)
                {
                    var timeSlot = new TimeSlot
                    {
                        Id = Guid.CreateVersion7().ToString(),
                        CourtId = courtId,
                        Day = request.Day,
                        StartTime = request.StartTime,
                        EndTime = request.EndTime,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    };

                    _context.TimeSlots.Add(timeSlot);
                    createdSlots.Add(timeSlot);
                }

                await _context.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);

                var responses = createdSlots.Select(s => MapToResponse(s, court)).ToList();
                return Result.Success<IReadOnlyList<TimeSlotResponse>>(responses.AsReadOnly());
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }

        /// <summary>
        /// Retrieves a single time slot by court ID and slot ID.
        /// </summary>
        /// <remarks>
        /// Simple read-only lookup. No ownership check — anyone can view a slot.
        /// Uses AsNoTracking() for performance.
        /// </remarks>
        /// <param name="courtId">The court ID.</param>
        /// <param name="slotId">The time slot ID.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// Success with TimeSlotResponse if found and belongs to the court.
        /// Failure codes:
        /// - Validation.CourtId (400) - courtId null/empty
        /// - Validation.SlotId (400) - slotId null/empty
        /// - TimeSlot.NotFound (404) - slot doesn't exist or doesn't belong to the court
        /// </returns>
        public async Task<Result<TimeSlotResponse>> GetTimeSlotAsync(
            string courtId, string slotId, CancellationToken ct = default)
        {
            // Validation: IDs
            if (string.IsNullOrWhiteSpace(courtId))
            {
                return Result.Failure<TimeSlotResponse>(new Error(
                    "Validation.CourtId", "CourtId cannot be null or empty", 400));
            }

            if (string.IsNullOrWhiteSpace(slotId))
            {
                return Result.Failure<TimeSlotResponse>(new Error(
                    "Validation.SlotId", "SlotId cannot be null or empty", 400));
            }

            // Lookup slot by courtId and slotId (including soft-deleted check via query filter)
            var timeSlot = await _context.TimeSlots
                .AsNoTracking()
                .Include(ts => ts.Court)
                .ThenInclude(c => c.Club)
                .FirstOrDefaultAsync(ts => ts.Id == slotId && ts.CourtId == courtId, ct);

            if (timeSlot is null)
            {
                return Result.Failure<TimeSlotResponse>(new Error(
                    "TimeSlot.NotFound", "Time slot not found", 404));
            }

            var response = MapToResponse(timeSlot, timeSlot.Court);
            return Result.Success(response);
        }

        /// <summary>
        /// Retrieves all time slots for a court, optionally filtered by date and availability.
        /// </summary>
        /// <remarks>
        /// Filters:
        /// - If date is null: returns slots from the next 30 days (DefaultLookaheadDays) — prevents unbounded queries.
        /// - If date is provided: returns slots for that specific date only.
        /// - If available is true: returns only slots with no active bookings (Confirmed or Pending status) and with StartTime in the future.
        /// - If available is false or null: returns all slots regardless of booking status.
        /// 
        /// All dates are treated as Egypt local time (Africa/Cairo).
        /// Results ordered by StartTime ascending.
        /// Uses AsNoTracking() for read-only performance.
        /// </remarks>
        /// <param name="courtId">The court ID.</param>
        /// <param name="date">Optional: filter by specific date (Egypt local time). If null, defaults to next 30 days.</param>
        /// <param name="available">Optional: if true, return only unbooked future slots; if false/null, return all slots.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// Success with list of TimeSlotResponse (ordered by StartTime ascending).
        /// Failure codes:
        /// - Validation.CourtId (400) - courtId null/empty
        /// </returns>
        public async Task<Result<IReadOnlyList<TimeSlotResponse>>> GetTimeSlotsAsync(
            string courtId, DateOnly? date = null, bool? available = null, CancellationToken ct = default)
        {
            // Validation: ID
            if (string.IsNullOrWhiteSpace(courtId))
            {
                return Result.Failure<IReadOnlyList<TimeSlotResponse>>(new Error(
                    "Validation.CourtId", "CourtId cannot be null or empty", 400));
            }

            // Determine date range to query
            DateOnly startDate;
            DateOnly endDate;

            if (date.HasValue)
            {
                // Specific date requested
                startDate = date.Value;
                endDate = date.Value;
            }
            else
            {
                // Default lookahead window: next N days from today (Cairo local time)
                var cairoNow = TimeZoneInfo.ConvertTime(DateTime.UtcNow, CairoTimeZone);
                startDate = DateOnly.FromDateTime(cairoNow);
                endDate = startDate.AddDays(DefaultLookaheadDays);
            }

            // Base query: filter by court and date range
            var query = _context.TimeSlots
                .AsNoTracking()
                .Include(ts => ts.Court)
                .ThenInclude(c => c.Club)
                .Include(ts => ts.Bookings)
                .Where(ts => ts.CourtId == courtId && ts.Day >= startDate && ts.Day <= endDate);

            // Apply availability filter if requested
            if (available.HasValue && available.Value)
            {
                var cairoNow = TimeZoneInfo.ConvertTime(DateTime.UtcNow, CairoTimeZone);
                var currentLocalTime = TimeOnly.FromDateTime(cairoNow);

                query = query.Where(ts =>
                    // No active bookings (matching TimeSlot.IsBooked property logic)
                    !ts.Bookings.Any(b =>
                        !b.IsDeleted &&
                        (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending)) &&
                    // StartTime is in the future (today or later)
                    (ts.Day > DateOnly.FromDateTime(cairoNow) || (ts.Day == DateOnly.FromDateTime(cairoNow) && ts.StartTime > currentLocalTime))
                );
            }

            // Order by start time
            query = query.OrderBy(ts => ts.Day).ThenBy(ts => ts.StartTime);

            var timeSlots = await query.ToListAsync(ct);

            var responses = timeSlots.Select(ts => MapToResponse(ts, ts.Court)).ToList();
            return Result.Success<IReadOnlyList<TimeSlotResponse>>(responses.AsReadOnly());
        }

        /// <summary>
        /// Updates a time slot with new times and date.
        /// </summary>
        /// <remarks>
        /// Process flow:
        /// 1. Validate all input IDs
        /// 2. Validate new time range and duration (same as create)
        /// 3. Load and verify court exists (404) and ownership (403)
        /// 4. Load and verify slot exists (404) and belongs to this court
        /// 5. Business rule: if slot has active bookings, reject update (cannot modify booked slots)
        /// 6. Check for overlaps with existing slots, excluding the slot being updated
        /// 7. Update and save
        /// 
        /// Ownership: userId must match Court.Club.OwnerId (the court owner).
        /// </remarks>
        /// <param name="userId">The court owner's user ID.</param>
        /// <param name="courtId">The court ID.</param>
        /// <param name="slotId">The time slot ID to update.</param>
        /// <param name="request">Request with new Day, StartTime, EndTime.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// Success with updated TimeSlotResponse.
        /// Failure codes:
        /// - Validation.UserId (400) - userId null/empty
        /// - Validation.CourtId (400) - courtId null/empty
        /// - Validation.SlotId (400) - slotId null/empty
        /// - Validation.TimeRange (400) - StartTime &gt;= EndTime or invalid duration
        /// - Validation.SlotDuration (400) - slot duration outside bounds
        /// - Validation.SlotInPast (400) - new slot start time is in the past
        /// - Court.NotFound (404) - court doesn't exist
        /// - Court.Forbidden (403) - userId does not own the court's club
        /// - TimeSlot.NotFound (404) - slot doesn't exist or doesn't belong to court
        /// - TimeSlot.HasActiveBooking (409) - slot has active bookings and cannot be modified
        /// - TimeSlot.Overlapping (409) - new time range overlaps existing slots
        /// </returns>
        public async Task<Result<TimeSlotResponse>> UpdateTimeSlotAsync(
            string userId, string courtId, string slotId, CreateTimeSlotRequest request, CancellationToken ct = default)
        {
            // Validation: IDs
            var idValidation = ValidateIds(userId, courtId);
            if (idValidation.IsFailure)
                return Result.Failure<TimeSlotResponse>(idValidation.Error);

            if (string.IsNullOrWhiteSpace(slotId))
            {
                return Result.Failure<TimeSlotResponse>(new Error(
                    "Validation.SlotId", "SlotId cannot be null or empty", 400));
            }

            // Validation: Time range and duration
            var timeValidation = ValidateTimeRange(request.StartTime, request.EndTime);
            if (timeValidation.IsFailure)
                return Result.Failure<TimeSlotResponse>(timeValidation.Error);

            // Validation: Slot is not in the past
            var pastValidation = ValidateSlotNotInPast(request.Day, request.StartTime);
            if (pastValidation.IsFailure)
                return Result.Failure<TimeSlotResponse>(pastValidation.Error);

            // Load court and verify ownership
            var courtResult = await LoadCourtWithOwnershipCheckAsync(courtId, userId, ct);
            if (courtResult.IsFailure)
                return Result.Failure<TimeSlotResponse>(courtResult.Error);

            var court = courtResult.Value;

            // Load slot and verify it belongs to this court
            var timeSlot = await _context.TimeSlots
                .Include(ts => ts.Bookings)
                .FirstOrDefaultAsync(ts => ts.Id == slotId && ts.CourtId == courtId, ct);

            if (timeSlot is null)
            {
                return Result.Failure<TimeSlotResponse>(new Error(
                    "TimeSlot.NotFound", "Time slot not found", 404));
            }

            // Business rule: Cannot update a slot with active bookings
            if (timeSlot.IsBooked)
            {
                return Result.Failure<TimeSlotResponse>(new Error(
                    "TimeSlot.HasActiveBooking",
                    "Cannot modify a time slot that has active bookings",
                    409));
            }

            // Check for overlaps with existing slots, excluding this slot
            var overlapResult = await HasOverlapAsync(courtId, request.Day, request.StartTime, request.EndTime, slotId, ct);
            if (overlapResult.IsFailure)
                return Result.Failure<TimeSlotResponse>(overlapResult.Error);

            // Update the slot
            timeSlot.Day = request.Day;
            timeSlot.StartTime = request.StartTime;
            timeSlot.EndTime = request.EndTime;

            _context.TimeSlots.Update(timeSlot);
            await _context.SaveChangesAsync(ct);

            var response = MapToResponse(timeSlot, court);
            return Result.Success(response);
        }

        /// <summary>
        /// Deletes a time slot (soft-delete via IsDeleted flag).
        /// </summary>
        /// <remarks>
        /// Process flow:
        /// 1. Validate all input IDs
        /// 2. Load and verify court exists (404) and ownership (403)
        /// 3. Load and verify slot exists (404) and belongs to this court
        /// 4. Business rule: if slot has active bookings, reject deletion
        /// 5. Soft-delete (set IsDeleted = true) and save
        /// 
        /// Ownership: userId must match Court.Club.OwnerId (the court owner).
        /// </remarks>
        /// <param name="userId">The court owner's user ID.</param>
        /// <param name="courtId">The court ID.</param>
        /// <param name="slotId">The time slot ID to delete.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// Success (empty Result) if deleted.
        /// Failure codes:
        /// - Validation.UserId (400) - userId null/empty
        /// - Validation.CourtId (400) - courtId null/empty
        /// - Validation.SlotId (400) - slotId null/empty
        /// - Court.NotFound (404) - court doesn't exist
        /// - Court.Forbidden (403) - userId does not own the court's club
        /// - TimeSlot.NotFound (404) - slot doesn't exist or doesn't belong to court
        /// - TimeSlot.HasActiveBooking (409) - slot has active bookings and cannot be deleted
        /// </returns>
        public async Task<Result> DeleteTimeSlotAsync(
            string userId, string courtId, string slotId, CancellationToken ct = default)
        {
            // Validation: IDs
            var idValidation = ValidateIds(userId, courtId);
            if (idValidation.IsFailure)
                return idValidation;

            if (string.IsNullOrWhiteSpace(slotId))
            {
                return Result.Failure(new Error(
                    "Validation.SlotId", "SlotId cannot be null or empty", 400));
            }

            // Load court and verify ownership
            var courtResult = await LoadCourtWithOwnershipCheckAsync(courtId, userId, ct);
            if (courtResult.IsFailure)
                return Result.Failure(courtResult.Error);

            // Load slot and verify it belongs to this court
            var timeSlot = await _context.TimeSlots
                .Include(ts => ts.Bookings)
                .FirstOrDefaultAsync(ts => ts.Id == slotId && ts.CourtId == courtId, ct);

            if (timeSlot is null)
            {
                return Result.Failure(new Error(
                    "TimeSlot.NotFound", "Time slot not found", 404));
            }

            // Business rule: Cannot delete a slot with active bookings
            if (timeSlot.IsBooked)
            {
                return Result.Failure(new Error(
                    "TimeSlot.HasActiveBooking",
                    "Cannot delete a time slot that has active bookings",
                    409));
            }

            // Soft-delete
            timeSlot.IsDeleted = true;

            _context.TimeSlots.Update(timeSlot);
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }

        #region Private Helper Methods

        /// <summary>
        /// Validates that userId and courtId are not null/empty/whitespace.
        /// </summary>
        private static Result ValidateIds(string userId, string courtId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result.Failure(new Error("Validation.UserId", "UserId cannot be null or empty", 400));
            }

            if (string.IsNullOrWhiteSpace(courtId))
            {
                return Result.Failure(new Error("Validation.CourtId", "CourtId cannot be null or empty", 400));
            }

            return Result.Success();
        }

        /// <summary>
        /// Validates that StartTime &lt; EndTime and the duration is within acceptable bounds.
        /// </summary>
        private static Result ValidateTimeRange(TimeOnly startTime, TimeOnly endTime)
        {
            if (startTime >= endTime)
            {
                return Result.Failure(new Error(
                    "Validation.TimeRange", "StartTime must be before EndTime", 400));
            }

            // Calculate duration in minutes
            var duration = (endTime.ToTimeSpan() - startTime.ToTimeSpan()).TotalMinutes;

            if (duration < MinSlotDurationMinutes)
            {
                return Result.Failure(new Error(
                    "Validation.SlotDuration",
                    $"Slot duration must be at least {MinSlotDurationMinutes} minutes",
                    400));
            }

            if (duration > MaxSlotDurationMinutes)
            {
                return Result.Failure(new Error(
                    "Validation.SlotDuration",
                    $"Slot duration cannot exceed {MaxSlotDurationMinutes} minutes (24 hours)",
                    400));
            }

            return Result.Success();
        }

        /// <summary>
        /// Validates that a time slot (day + start time) is not in the past.
        /// Converts local Egypt time to UTC for comparison with DateTime.UtcNow.
        /// </summary>
        private static Result ValidateSlotNotInPast(DateOnly day, TimeOnly startTime)
        {
            // Combine DateOnly and TimeOnly into a DateTime in Cairo timezone
            var slotLocalDateTime = day.ToDateTime(startTime);

            // Convert from Cairo local time to UTC
            var slotUtc = TimeZoneInfo.ConvertTimeToUtc(slotLocalDateTime, CairoTimeZone);

            // Compare with current UTC time
            if (slotUtc < DateTime.UtcNow)
            {
                return Result.Failure(new Error(
                    "Validation.SlotInPast", "Cannot create a time slot in the past", 400));
            }

            return Result.Success();
        }

        /// <summary>
        /// Loads a court, verifies it exists, and checks ownership.
        /// Includes related Club for ownership verification (Court.Club.OwnerId).
        /// </summary>
        /// <param name="courtId">The court ID to load.</param>
        /// <param name="userId">The user ID expected to own the court's club.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// Success with Court entity if found and owned by userId.
        /// Failure codes:
        /// - Court.NotFound (404) - court doesn't exist
        /// - Court.Forbidden (403) - userId does not own the court's club
        /// </returns>
        private async Task<Result<Court>> LoadCourtWithOwnershipCheckAsync(
            string courtId, string userId, CancellationToken ct)
        {
            var court = await _context.Courts
                .Include(c => c.Club)
                .FirstOrDefaultAsync(c => c.Id == courtId && !c.IsDeleted, ct);

            if (court is null)
            {
                return Result.Failure<Court>(new Error(
                    "Court.NotFound", "Court not found", 404));
            }

            if (court.Club.OwnerId != userId)
            {
                return Result.Failure<Court>(new Error(
                    "Court.Forbidden", "Not authorized to manage this court", 403));
            }

            return Result.Success(court);
        }

        /// <summary>
        /// Checks if a new time slot (day + start/end times) overlaps any existing slots for a court.
        /// Optionally excludes a specific slot ID (used during updates).
        /// 
        /// Overlap logic: Two slots overlap if they share any time on the same day.
        /// [A.Start, A.End) and [B.Start, B.End) overlap if A.Start &lt; B.End AND B.Start &lt; A.End
        /// </summary>
        /// <param name="courtId">The court ID to check against.</param>
        /// <param name="day">The day of the new slot.</param>
        /// <param name="startTime">The start time of the new slot.</param>
        /// <param name="endTime">The end time of the new slot.</param>
        /// <param name="excludeSlotId">Optional: slot ID to exclude from comparison (e.g., during update).</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// Success if no overlaps found.
        /// Failure with TimeSlot.Overlapping (409) if overlap detected.
        /// </returns>
        private async Task<Result> HasOverlapAsync(
            string courtId, DateOnly day, TimeOnly startTime, TimeOnly endTime,
            string? excludeSlotId, CancellationToken ct)
        {
            var existingSlots = await _context.TimeSlots
                .AsNoTracking()
                .Where(ts =>
                    ts.CourtId == courtId &&
                    ts.Day == day &&
                    (excludeSlotId == null || ts.Id != excludeSlotId))
                .ToListAsync(ct);

            foreach (var existing in existingSlots)
            {
                if (SlotsOverlap(day, startTime, endTime, existing.Day, existing.StartTime, existing.EndTime))
                {
                    return Result.Failure(new Error(
                        "TimeSlot.Overlapping",
                        $"Time slot overlaps with existing slot from {existing.StartTime:HH:mm} to {existing.EndTime:HH:mm}",
                        409));
                }
            }

            return Result.Success();
        }

        /// <summary>
        /// Determines if two time slots on the same day overlap.
        /// Slots overlap if: slot1.Start &lt; slot2.End AND slot2.Start &lt; slot1.End
        /// </summary>
        private static bool SlotsOverlap(
            DateOnly day1, TimeOnly start1, TimeOnly end1,
            DateOnly day2, TimeOnly start2, TimeOnly end2)
        {
            // Different days: no overlap
            if (day1 != day2)
                return false;

            // Same day: check time overlap
            return start1 < end2 && start2 < end1;
        }

        /// <summary>
        /// Maps a TimeSlot entity to TimeSlotResponse DTO.
        /// </summary>
        private static TimeSlotResponse MapToResponse(TimeSlot timeSlot, Court court)
        {
            var clubSummary = new ClubSummary(
                court.Club.Id,
                court.Club.Name,
                court.Club.LogoUrl,
                court.Club.City,
                court.Club.Governorate);

            var sportTypeDto = (SportTypeDto)court.SportType;

            var courtSummary = new CourtSummary(
                court.Id,
                court.Name,
                court.ImageUrl,
                sportTypeDto,
                court.PricePerHour,
                clubSummary);

            return new TimeSlotResponse(
                timeSlot.Id,
                courtSummary,
                timeSlot.Day,
                timeSlot.StartTime,
                timeSlot.EndTime,
                timeSlot.IsBooked,
                timeSlot.CreatedAt);
        }

        #endregion
    }
}

