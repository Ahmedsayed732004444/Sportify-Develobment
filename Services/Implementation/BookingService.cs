using Sportiva.Abstractions;
using Sportiva.Contracts.Bookings;
using Sportiva.Contracts.Common;
using Sportiva.Contracts.Shared.Enums;
using Sportiva.Contracts.Shared.Summaries;
using Sportiva.Entities;
using Sportiva.Enums;

namespace Sportiva.Services.Implementation
{
    /// <summary>
    /// Service for managing court bookings with full business logic, payment integration,
    /// ownership verification (three contexts: booking owner, club owner, or dual-access),
    /// and race condition prevention via database-level unique constraint on active bookings.
    /// </summary>
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWalletService _walletService;

        // Cancellation refund policy constants
        private const int FullRefundCutoffHours = 24;      // Full refund if cancelled >24 hours before slot start
        private const int PartialRefundCutoffHours = 1;    // 50% refund if cancelled 1-24 hours before
        private const decimal PartialRefundPercentage = 0.5m; // Refund percentage in the partial window
        private const int CancellationBlockedWithinHours = 1; // Block cancellation if <1 hour before (instead of zero refund)

        public BookingService(ApplicationDbContext context, IWalletService walletService)
        {
            _context = context;
            _walletService = walletService;
        }

        /// <summary>
        /// Creates a new booking for a customer on a specific time slot.
        /// </summary>
        /// <remarks>
        /// Process flow:
        /// 1. Validate all input IDs
        /// 2. Load and verify TimeSlot exists, is not in the past, and belongs to the given Court
        /// 3. Verify Court and Club are active
        /// 4. Compute price from Court.PricePerHour (never trust client input)
        /// 5. Wrap in a transaction: deduct payment from wallet FIRST, then insert Booking row
        /// 6. If unique index violation occurs (race lost), catch and translate to TimeSlot.AlreadyBooked (409)
        /// 7. On any failure, roll back the wallet deduction to ensure atomicity
        /// 
        /// Race condition prevention: A unique index on (TimeSlotId) filtered to only active booking
        /// statuses (Pending, Confirmed) ensures at the database level that only one booking can
        /// claim a slot, regardless of concurrent requests.
        /// </remarks>
        /// <param name="userId">The customer's user ID (becomes Booking.UserId).</param>
        /// <param name="request">Request containing CourtId and TimeSlotId.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// Success with BookingResponse if booking created and payment processed.
        /// Failure codes:
        /// - Validation.UserId (400) - userId null/empty
        /// - Validation.CourtId (400) - courtId null/empty
        /// - Validation.TimeSlotId (400) - timeSlotId null/empty
        /// - Court.NotFound (404) - court doesn't exist
        /// - Court.Inactive (403) - court is not active
        /// - Club.Inactive (403) - court's parent club is not active
        /// - TimeSlot.NotFound (404) - time slot doesn't exist or doesn't belong to the court
        /// - TimeSlot.InThePast (400) - slot start time is already past
        /// - TimeSlot.AlreadyBooked (409) - slot already has an active booking (unique index violation)
        /// - Wallet.InsufficientBalance (402) - customer has insufficient wallet balance
        /// </returns>
        public async Task<Result<BookingResponse>> CreateBookingAsync(
            string userId, CreateBookingRequest request, CancellationToken ct = default)
        {
            // Validation: IDs
            var idValidation = ValidateIds(userId, request.CourtId, request.TimeSlotId);
            if (idValidation.IsFailure)
                return Result.Failure<BookingResponse>(idValidation.Error);

            // Load TimeSlot with related Court and Club
            var timeSlot = await _context.TimeSlots
                .Include(ts => ts.Court)
                .ThenInclude(c => c.Club)
                .FirstOrDefaultAsync(ts => ts.Id == request.TimeSlotId && ts.CourtId == request.CourtId, ct);

            if (timeSlot is null)
            {
                return Result.Failure<BookingResponse>(new Error(
                    "TimeSlot.NotFound", "Time slot not found or doesn't belong to the specified court", 404));
            }

            // Verify slot is not in the past
            var slotDateTime = CombineSlotDateTimeInUtc(timeSlot);
            if (slotDateTime <= DateTime.UtcNow)
            {
                return Result.Failure<BookingResponse>(new Error(
                    "TimeSlot.InThePast", "Cannot book a time slot that has already started", 400));
            }

            // Verify Court and Club are active
            if (!timeSlot.Court.IsActive)
            {
                return Result.Failure<BookingResponse>(new Error(
                    "Court.Inactive", "The court is not active", 403));
            }

            if (!timeSlot.Court.Club.IsActive)
            {
                return Result.Failure<BookingResponse>(new Error(
                    "Club.Inactive", "The club is not active", 403));
            }

            // Compute price from Court
            var bookingPrice = timeSlot.EndTime.Hour - timeSlot.StartTime.Hour == 0
                ? timeSlot.Court.PricePerHour
                : timeSlot.Court.PricePerHour * (timeSlot.EndTime.Hour - timeSlot.StartTime.Hour);

            // Deduct payment from wallet BEFORE creating the booking
            var deductResult = await _walletService.DeductAsync(
                userId, bookingPrice, $"Booking.CreateBooking for {timeSlot.Court.Name ?? request.CourtId}", ct);

            if (deductResult.IsFailure)
            {
                return Result.Failure<BookingResponse>(deductResult.Error);
            }

            // Create booking in a transaction
            using var transaction = await _context.Database.BeginTransactionAsync(ct);
            try
            {
                var booking = new Booking
                {
                    Id = Guid.CreateVersion7().ToString(),
                    CourtId = request.CourtId,
                    UserId = userId,
                    TimeSlotId = request.TimeSlotId,
                    BookingDate = DateTime.UtcNow,
                    Price = bookingPrice,
                    Status = BookingStatus.Confirmed, // Synchronous wallet charge, so mark as confirmed immediately
                    IsDeleted = false
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync(ct);

                await transaction.CommitAsync(ct);

                // Map to response
                booking.TimeSlot = timeSlot;
                booking.Court = timeSlot.Court;

                using var scope = new MapContextScope();
                scope.Context.Parameters["currentUserId"] = userId;
                var response = booking.Adapt<BookingResponse>();
                return Result.Success(response);
            }
            catch (DbUpdateException dbEx) when (dbEx.InnerException?.Message.Contains("UNIQUE constraint", StringComparison.OrdinalIgnoreCase) == true)
            {
                // Unique index violation: another booking won the race for this slot
                await transaction.RollbackAsync(ct);

                // Refund the wallet deduction
                await _walletService.CreditAsync(
                    userId, bookingPrice, "Booking.CreateBooking race condition refund", ct);

                return Result.Failure<BookingResponse>(new Error(
                    "TimeSlot.AlreadyBooked", "This time slot has already been booked by another customer", 409));
            }
            catch
            {
                await transaction.RollbackAsync(ct);

                // Refund the wallet deduction on any other failure
                await _walletService.CreditAsync(
                    userId, bookingPrice, "Booking.CreateBooking transaction rollback", ct);

                throw;
            }
        }

        /// <summary>
        /// Retrieves a single booking by ID with dual-access control.
        /// </summary>
        /// <remarks>
        /// Access rules: viewable by EITHER the booking owner (Booking.UserId == currentUserId)
        /// OR the club owner (Court.Club.OwnerId == currentUserId). This allows:
        /// - Customers to view their own bookings
        /// - Club/venue owners to look up any booking on their venue (e.g., handling disputes)
        /// </remarks>
        /// <param name="bookingId">The booking ID to retrieve.</param>
        /// <param name="currentUserId">The user ID requesting the booking (used for access control).</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// Success with BookingResponse if found and access is permitted.
        /// Failure codes:
        /// - Validation.BookingId (400) - bookingId null/empty
        /// - Validation.UserId (400) - currentUserId null/empty
        /// - Booking.NotFound (404) - booking doesn't exist
        /// - Booking.Forbidden (403) - booking exists but currentUserId is neither the owner nor club owner
        /// </returns>
        public async Task<Result<BookingResponse>> GetBookingAsync(
            string bookingId, string currentUserId, CancellationToken ct = default)
        {
            // Validation: IDs
            if (string.IsNullOrWhiteSpace(bookingId))
            {
                return Result.Failure<BookingResponse>(new Error(
                    "Validation.BookingId", "BookingId cannot be null or empty", 400));
            }

            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Result.Failure<BookingResponse>(new Error(
                    "Validation.UserId", "UserId cannot be null or empty", 400));
            }

            // Load booking with all related entities
            var booking = await _context.Bookings
                .AsNoTracking()
                .Include(b => b.Court)
                .ThenInclude(c => c.Club)
                .Include(b => b.TimeSlot)
                .FirstOrDefaultAsync(b => b.Id == bookingId, ct);

            if (booking is null)
            {
                return Result.Failure<BookingResponse>(new Error(
                    "Booking.NotFound", "Booking not found", 404));
            }

            // Dual-access check: booking owner OR club owner
            var isBookingOwner = booking.UserId == currentUserId;
            var isClubOwner = booking.Court.Club.OwnerId == currentUserId;

            if (!isBookingOwner && !isClubOwner)
            {
                return Result.Failure<BookingResponse>(new Error(
                    "Booking.Forbidden", "Not authorized to view this booking", 403));
            }

            using var scope = new MapContextScope();
            scope.Context.Parameters["currentUserId"] = currentUserId;
            var response = booking.Adapt<BookingResponse>();
            return Result.Success(response);
        }

        /// <summary>
        /// Retrieves the receipt for a booking (customer-facing proof of payment).
        /// </summary>
        /// <remarks>
        /// Only the booking owner (customer) can view the receipt — club owners cannot.
        /// This is a financial document showing the customer what they paid for.
        /// </remarks>
        /// <param name="userId">The requesting user ID (must be the booking owner).</param>
        /// <param name="bookingId">The booking ID to retrieve receipt for.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// Success with BookingResponse (with financial details) if access permitted.
        /// Failure codes:
        /// - Validation.UserId (400) - userId null/empty
        /// - Validation.BookingId (400) - bookingId null/empty
        /// - Booking.NotFound (404) - booking doesn't exist
        /// - Booking.Forbidden (403) - booking exists but userId is not the booking owner
        /// </returns>
        public async Task<Result<BookingResponse>> GetBookingReceiptAsync(
            string userId, string bookingId, CancellationToken ct = default)
        {
            // Validation: IDs
            var idValidation = ValidateIds(userId, null!, bookingId);
            if (idValidation.IsFailure)
                return Result.Failure<BookingResponse>(idValidation.Error);

            // Load booking with all related entities
            var booking = await _context.Bookings
                .AsNoTracking()
                .Include(b => b.Court)
                .ThenInclude(c => c.Club)
                .Include(b => b.TimeSlot)
                .FirstOrDefaultAsync(b => b.Id == bookingId, ct);

            if (booking is null)
            {
                return Result.Failure<BookingResponse>(new Error(
                    "Booking.NotFound", "Booking not found", 404));
            }

            // Only the booking owner can view the receipt
            if (booking.UserId != userId)
            {
                return Result.Failure<BookingResponse>(new Error(
                    "Booking.Forbidden", "Not authorized to view this receipt", 403));
            }

            using var scope = new MapContextScope();
            scope.Context.Parameters["currentUserId"] = userId;
            var response = booking.Adapt<BookingResponse>();
            return Result.Success(response);
        }

        /// <summary>
        /// Cancels a booking and processes a refund based on cancellation timing policy.
        /// </summary>
        /// <remarks>
        /// Refund policy:
        /// - >24 hours before slot start: 100% refund
        /// - 1-24 hours before: 50% refund
        /// - &lt;1 hour before or slot already started: CANNOT CANCEL (Booking.CancellationWindowClosed)
        /// 
        /// After cancellation, the TimeSlot automatically becomes available for rebooking
        /// because the unique index only counts Pending and Confirmed statuses.
        /// </remarks>
        /// <param name="userId">The requesting user ID (must be the booking owner).</param>
        /// <param name="bookingId">The booking ID to cancel.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// Success (empty Result) if cancellation processed.
        /// Failure codes:
        /// - Validation.UserId (400) - userId null/empty
        /// - Validation.BookingId (400) - bookingId null/empty
        /// - Booking.NotFound (404) - booking doesn't exist
        /// - Booking.Forbidden (403) - booking exists but userId is not the owner
        /// - Booking.AlreadyCancelled (409) - booking is already in Cancelled status
        /// - Booking.Completed (409) - booking is in Completed status (can't cancel past events)
        /// - Booking.CancellationWindowClosed (409) - cancellation requested within 1 hour of slot start
        /// </returns>
        public async Task<Result> CancelBookingAsync(
            string userId, string bookingId, CancellationToken ct = default)
        {
            // Validation: IDs
            var idValidation = ValidateIds(userId, null!, bookingId);
            if (idValidation.IsFailure)
                return idValidation;

            // Load booking with related entities
            var booking = await _context.Bookings
                .Include(b => b.TimeSlot)
                .Include(b => b.Court)
                .ThenInclude(c => c.Club)
                .FirstOrDefaultAsync(b => b.Id == bookingId, ct);

            if (booking is null)
            {
                return Result.Failure(new Error(
                    "Booking.NotFound", "Booking not found", 404));
            }

            // Ownership check
            if (booking.UserId != userId)
            {
                return Result.Failure(new Error(
                    "Booking.Forbidden", "Not authorized to cancel this booking", 403));
            }

            // Status checks
            if (booking.Status == BookingStatus.Cancelled)
            {
                return Result.Failure(new Error(
                    "Booking.AlreadyCancelled", "Booking is already cancelled", 409));
            }

            if (booking.Status == BookingStatus.Completed)
            {
                return Result.Failure(new Error(
                    "Booking.Completed", "Cannot cancel a completed booking", 409));
            }

            // Check if cancellation window is still open
            var slotDateTime = CombineSlotDateTimeInUtc(booking.TimeSlot);
            var hoursUntilSlot = (slotDateTime - DateTime.UtcNow).TotalHours;

            if (hoursUntilSlot < CancellationBlockedWithinHours)
            {
                return Result.Failure(new Error(
                    "Booking.CancellationWindowClosed",
                    $"Cannot cancel within {CancellationBlockedWithinHours} hour(s) of slot start",
                    409));
            }

            // Calculate refund amount
            var refundAmount = CalculateRefund(hoursUntilSlot, booking.Price);

            // Update booking to cancelled state
            booking.Status = BookingStatus.Cancelled;

            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync(ct);

            // Process refund if applicable
            if (refundAmount > 0)
            {
                await _walletService.CreditAsync(
                    userId,
                    refundAmount,
                    $"Booking.Cancel refund from {booking.Court?.Name ?? booking.CourtId}",
                    ct);
            }

            return Result.Success();
        }

        /// <summary>
        /// Retrieves paginated bookings for the requesting customer.
        /// </summary>
        /// <remarks>
        /// Returns only bookings owned by the userId, no ownership check needed.
        /// Optionally filters by booking status (Pending, Confirmed, Cancelled, Completed).
        /// Results ordered by TimeSlot StartTime ascending (upcoming bookings first).
        /// </remarks>
        /// <param name="userId">The requesting customer's user ID.</param>
        /// <param name="filters">Pagination and sort parameters.</param>
        /// <param name="status">Optional: filter by booking status.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// Success with paginated list of BookingResponse.
        /// Failure codes:
        /// - Validation.UserId (400) - userId null/empty
        /// - Validation.Filters (400) - invalid pagination parameters
        /// </returns>
        public async Task<Result<PaginatedList<BookingResponse>>> GetMyBookingsAsync(
            string userId, RequestFilters filters, BookingStatus? status = null, CancellationToken ct = default)
        {
            // Validation: ID
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result.Failure<PaginatedList<BookingResponse>>(new Error(
                    "Validation.UserId", "UserId cannot be null or empty", 400));
            }

            // Validate filters
            if (filters.PageNumber < 1)
            {
                return Result.Failure<PaginatedList<BookingResponse>>(new Error(
                    "Validation.Filters", "PageNumber must be >= 1", 400));
            }

            if (filters.PageSize < 1 || filters.PageSize > 50)
            {
                return Result.Failure<PaginatedList<BookingResponse>>(new Error(
                    "Validation.Filters", "PageSize must be between 1 and 50", 400));
            }

            // Build query
            // Build query (Clean and simple)
            var query = _context.Bookings
                .AsNoTracking()
                .Where(b => b.UserId == userId);

            // Apply status filter if provided (Now it will work perfectly without errors)
            if (status.HasValue)
            {
                query = query.Where(b => b.Status == status.Value);
            }

            // Order: upcoming first (ascending StartTime)
            query = query.OrderBy(b => b.TimeSlot.Day).ThenBy(b => b.TimeSlot.StartTime);

            using var scope = new MapContextScope();
            scope.Context.Parameters["currentUserId"] = userId;

            // Project and paginate
            var projectedQuery = query.ProjectToType<BookingResponse>();
            var result = await PaginatedList<BookingResponse>.CreateAsync(
                projectedQuery, filters.PageNumber, filters.PageSize, ct);

            return Result.Success(result);
        }

        /// <summary>
        /// Retrieves all bookings for a specific court (club owner view).
        /// </summary>
        /// <remarks>
        /// Club owner only: userId must match Court.Club.OwnerId.
        /// Returns all bookings on the court, paginated.
        /// Optionally filters by specific date (matches TimeSlot.Day).
        /// Results ordered by TimeSlot StartTime ascending (upcoming slots first).
        /// </remarks>
        /// <param name="userId">The requesting user ID (must be the court's club owner).</param>
        /// <param name="courtId">The court ID to retrieve bookings for.</param>
        /// <param name="filters">Pagination and sort parameters.</param>
        /// <param name="date">Optional: filter by specific date.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// Success with paginated list of BookingResponse.
        /// Failure codes:
        /// - Validation.UserId (400) - userId null/empty
        /// - Validation.CourtId (400) - courtId null/empty
        /// - Validation.Filters (400) - invalid pagination parameters
        /// - Court.NotFound (404) - court doesn't exist
        /// - Court.Forbidden (403) - userId is not the court's club owner
        /// </returns>
        public async Task<Result<PaginatedList<BookingResponse>>> GetCourtBookingsAsync(
            string userId, string courtId, RequestFilters filters, DateOnly? date = null, CancellationToken ct = default)
        {
            // Validation: IDs
            var idValidation = ValidateIds(userId, courtId, null!);
            if (idValidation.IsFailure)
                return Result.Failure<PaginatedList<BookingResponse>>(idValidation.Error);

            // Validate filters
            if (filters.PageNumber < 1 || filters.PageSize < 1 || filters.PageSize > 50)
            {
                return Result.Failure<PaginatedList<BookingResponse>>(new Error(
                    "Validation.Filters", "Invalid pagination parameters", 400));
            }

            // Load court and verify ownership
            var courtResult = await LoadCourtWithOwnershipCheckAsync(courtId, userId, ct);
            if (courtResult.IsFailure)
                return Result.Failure<PaginatedList<BookingResponse>>(courtResult.Error);

            var court = courtResult.Value;

            // Build query
            var query = _context.Bookings
                .AsNoTracking()
                .Where(b => b.CourtId == courtId);
            
            // Apply date filter if provided
            if (date.HasValue)
            {
                query = query.Where(b => b.TimeSlot.Day == date.Value);
            }

            // Order: upcoming slots first (ascending StartTime)
            query = query.OrderBy(b => b.TimeSlot.Day).ThenBy(b => b.TimeSlot.StartTime);

            using var scope = new MapContextScope();
            scope.Context.Parameters["currentUserId"] = userId;

            // Project and paginate
            var projectedQuery = query.ProjectToType<BookingResponse>();
            var result = await PaginatedList<BookingResponse>.CreateAsync(
                projectedQuery, filters.PageNumber, filters.PageSize, ct);

            return Result.Success(result);
        }

        /// <summary>
        /// Retrieves all bookings for a specific club (club owner view).
        /// </summary>
        /// <remarks>
        /// Club owner only: userId must match Club.OwnerId.
        /// Returns all bookings across all courts in the club, paginated.
        /// Results ordered by TimeSlot StartTime ascending (upcoming slots first).
        /// </remarks>
        /// <param name="userId">The requesting user ID (must be the club owner).</param>
        /// <param name="clubId">The club ID to retrieve bookings for.</param>
        /// <param name="filters">Pagination and sort parameters.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// Success with paginated list of BookingResponse.
        /// Failure codes:
        /// - Validation.UserId (400) - userId null/empty
        /// - Validation.ClubId (400) - clubId null/empty
        /// - Validation.Filters (400) - invalid pagination parameters
        /// - Club.NotFound (404) - club doesn't exist
        /// - Club.Forbidden (403) - userId is not the club owner
        /// </returns>
        public async Task<Result<PaginatedList<BookingResponse>>> GetClubBookingsAsync(
            string userId, string clubId, RequestFilters filters, CancellationToken ct = default)
        {
            // Validation: IDs
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result.Failure<PaginatedList<BookingResponse>>(new Error(
                    "Validation.UserId", "UserId cannot be null or empty", 400));
            }

            if (string.IsNullOrWhiteSpace(clubId))
            {
                return Result.Failure<PaginatedList<BookingResponse>>(new Error(
                    "Validation.ClubId", "ClubId cannot be null or empty", 400));
            }

            // Validate filters
            if (filters.PageNumber < 1 || filters.PageSize < 1 || filters.PageSize > 50)
            {
                return Result.Failure<PaginatedList<BookingResponse>>(new Error(
                    "Validation.Filters", "Invalid pagination parameters", 400));
            }

            // Load club and verify ownership
            var clubResult = await LoadClubWithOwnershipCheckAsync(clubId, userId, ct);
            if (clubResult.IsFailure)
                return Result.Failure<PaginatedList<BookingResponse>>(clubResult.Error);

            // Build query: all bookings across all courts in the club
            var query = _context.Bookings
                .AsNoTracking()
                .Where(b => b.Court.ClubId == clubId);
                

            // Order: upcoming slots first (ascending StartTime)
            query = query.OrderBy(b => b.TimeSlot.Day).ThenBy(b => b.TimeSlot.StartTime);

            using var scope = new MapContextScope();
            scope.Context.Parameters["currentUserId"] = userId;

            // Project and paginate
            var projectedQuery = query.ProjectToType<BookingResponse>();
            var result = await PaginatedList<BookingResponse>.CreateAsync(
                projectedQuery, filters.PageNumber, filters.PageSize, ct);

            return Result.Success(result);
        }

        #region Private Helper Methods

        /// <summary>
        /// Validates that userId, courtId, and timeSlotId are not null/empty/whitespace.
        /// Allows null values to be passed; only validates non-null parameters.
        /// </summary>
        private static Result ValidateIds(string? userId, string? courtId, string? timeSlotId)
        {
            if (userId != null && string.IsNullOrWhiteSpace(userId))
            {
                return Result.Failure(new Error("Validation.UserId", "UserId cannot be null or empty", 400));
            }

            if (courtId != null && string.IsNullOrWhiteSpace(courtId))
            {
                return Result.Failure(new Error("Validation.CourtId", "CourtId cannot be null or empty", 400));
            }

            if (timeSlotId != null && string.IsNullOrWhiteSpace(timeSlotId))
            {
                return Result.Failure(new Error("Validation.TimeSlotId", "TimeSlotId cannot be null or empty", 400));
            }

            return Result.Success();
        }

        /// <summary>
        /// Loads a court and verifies the requesting user owns the court's parent club.
        /// </summary>
        private async Task<Result<Court>> LoadCourtWithOwnershipCheckAsync(
            string courtId, string userId, CancellationToken ct)
        {
            var court = await _context.Courts
                .AsNoTracking()
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
                    "Court.Forbidden", "Not authorized to view bookings for this court", 403));
            }

            return Result.Success(court);
        }

        /// <summary>
        /// Loads a club and verifies the requesting user is the club owner.
        /// </summary>
        private async Task<Result<Club>> LoadClubWithOwnershipCheckAsync(
            string clubId, string userId, CancellationToken ct)
        {
            var club = await _context.Clubs
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == clubId && !c.IsDeleted, ct);

            if (club is null)
            {
                return Result.Failure<Club>(new Error(
                    "Club.NotFound", "Club not found", 404));
            }

            if (club.OwnerId != userId)
            {
                return Result.Failure<Club>(new Error(
                    "Club.Forbidden", "Not authorized to view bookings for this club", 403));
            }

            return Result.Success(club);
        }

        /// <summary>
        /// Calculates refund amount based on hours until slot start and refund policy.
        /// Policy:
        /// - >24 hours: 100% refund
        /// - 1-24 hours: 50% refund
        /// - <1 hour: already blocked by caller, but return 0 if this is somehow called
        /// </summary>
        private static decimal CalculateRefund(double hoursUntilSlot, decimal bookingPrice)
        {
            if (hoursUntilSlot >= FullRefundCutoffHours)
            {
                return bookingPrice; // 100% refund
            }

            if (hoursUntilSlot >= PartialRefundCutoffHours)
            {
                return bookingPrice * PartialRefundPercentage; // 50% refund
            }

            // Already blocked by caller, but return 0 if somehow reached
            return 0m;
        }

        /// <summary>
        /// Combines TimeSlot's DateOnly and TimeOnly into a UTC DateTime for comparison.
        /// Assumes DateOnly/TimeOnly are stored as Egypt local time (Africa/Cairo).
        /// </summary>
        private static DateTime CombineSlotDateTimeInUtc(TimeSlot slot)
        {
            // Combine DateOnly and TimeOnly into local DateTime
            var localDateTime = slot.Day.ToDateTime(slot.StartTime);

            // Convert from Cairo local time to UTC
            var cairoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
            var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(localDateTime, cairoTimeZone);

            return utcDateTime;
        }



        #endregion
    }
}
