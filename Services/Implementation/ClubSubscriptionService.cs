using Sportiva.Abstractions;
using Sportiva.Contracts.Common;
using Sportiva.Contracts.Shared.Enums;
using Sportiva.Contracts.Shared.Summaries;
using Sportiva.Contracts.Subscriptions;
using Sportiva.Enums;
using Sportiva.Extensions;
using Sportiva.Persistence;

namespace Sportiva.Services.Implementation
{
    /// <summary>
    /// Service for managing club subscriptions with full business logic hardening,
    /// payment integration, refund calculations, and edge-case handling.
    /// </summary>
    public class ClubSubscriptionService : IClubSubscriptionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWalletService _walletService;

        // Named constants for business rules
        private const int MinSubscriptionDays = 1;
        private const int MaxSubscriptionDays = 365; // 1 year max per subscription
        private const int NonRefundablePeriodDays = 3; // No refund in last 3 days
        private const decimal MinimumRefundAmount = 0.01m;

        public ClubSubscriptionService(ApplicationDbContext context, IWalletService walletService)
{
            _context = context;
            _walletService = walletService;
        }

        /// <summary>
        /// Subscribes a user to a club with a selected plan.
        /// </summary>
        /// <remarks>
        /// Process flow:
        /// 1. Validate all input IDs and dates
        /// 2. Load and validate Plan and Club exist and are active
        /// 3. Check for existing active subscriptions for this user+club combo
        /// 4. Calculate price from plan
        /// 5. Deduct payment from wallet (fails gracefully if insufficient balance)
        /// 6. Create subscription record and payment record in a transaction
        /// </remarks>
        /// <param name="userId">The subscribing user's ID.</param>
        /// <param name="clubId">The club ID to subscribe to.</param>
        /// <param name="request">Request with PlanId, StartDate, EndDate.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// Success with ClubSubscriptionResponse if subscription created and payment processed.
        /// Failure codes:
        /// - Validation.UserId (400) - userId null/empty
        /// - Validation.ClubId (400) - clubId null/empty
        /// - Validation.PlanId (400) - planId null/empty
        /// - Validation.DateRange (400) - invalid date range or duration out of bounds
        /// - Club.NotFound (404) - club doesn't exist
        /// - Club.Inactive (403) - club is not active
        /// - Plan.NotFound (404) - plan doesn't exist
        /// - Plan.Inactive (403) - plan is not active
        /// - Subscription.DuplicateActive (409) - user already has active subscription for this club
        /// - Wallet.InsufficientBalance (402) - insufficient funds to pay for subscription
        /// </returns>
        public async Task<Result<ClubSubscriptionResponse>> SubscribeAsync(
            string userId, string clubId, CreateClubSubscriptionRequest request, CancellationToken ct = default)
        {
            // Validation: IDs
            var idValidation = ValidateIds(userId, clubId);
            if (idValidation.IsFailure)
                return Result.Failure<ClubSubscriptionResponse>(idValidation.Error);

            if (string.IsNullOrWhiteSpace(request.PlanId))
    {
                return Result.Failure<ClubSubscriptionResponse>(new Error(
                    "Validation.PlanId", "PlanId cannot be null or empty", 400));
            }

            // Validation: Date range
            var dateValidation = ValidateDateRange(request.StartDate, request.EndDate);
            if (dateValidation.IsFailure)
                return Result.Failure<ClubSubscriptionResponse>(dateValidation.Error);

            // Load Club (existence + active check)
            var club = await _context.Clubs
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == clubId && !c.IsDeleted, ct);

            if (club is null)
            {
                return Result.Failure<ClubSubscriptionResponse>(new Error(
                    "Club.NotFound", "Club not found", 404));
            }

            if (!club.IsActive)
            {
                return Result.Failure<ClubSubscriptionResponse>(new Error(
                    "Club.Inactive", "Cannot subscribe to an inactive club", 403));
            }

            // Load Plan (existence + active check)
            var plan = await _context.SubscriptionPlans
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.PlanId && !p.IsDeleted, ct);

            if (plan is null)
            {
                return Result.Failure<ClubSubscriptionResponse>(new Error(
                    "Plan.NotFound", "Subscription plan not found", 404));
            }

            if (!plan.IsActive)
            {
                return Result.Failure<ClubSubscriptionResponse>(new Error(
                    "Plan.Inactive", "Cannot subscribe to an inactive plan", 403));
            }

            // Check for existing active subscription (duplicate prevention)
            var existingActive = await _context.ClubSubscriptions
                .AsNoTracking()
                .FirstOrDefaultAsync(s =>
                    s.UserId == userId &&
                    s.ClubId == clubId &&
                    s.Status == SubscriptionStatus.Active &&
                    !s.IsDeleted, ct);

            if (existingActive is not null)
            {
                return Result.Failure<ClubSubscriptionResponse>(new Error(
                    "Subscription.DuplicateActive",
                    "User already has an active subscription for this club",
                    409));
            }

            // Calculate price server-side (never trust client input)
            var durationDays = (request.EndDate.Date - request.StartDate.Date).Days + 1;
            var dailyRate = plan.MonthlyPrice / 30m; // Approximate: 30 days/month
            var calculatedPrice = dailyRate * durationDays;

            // Deduct payment from wallet BEFORE creating subscription
            var deductResult = await _walletService.DeductAsync(
                userId, calculatedPrice, $"Subscription.Subscribe to {club.Name ?? clubId}", ct);

            if (deductResult.IsFailure)
            {
                return Result.Failure<ClubSubscriptionResponse>(deductResult.Error);
            }

            // Create subscription and payment in a transaction
            using var transaction = await _context.Database.BeginTransactionAsync(ct);
        try
        {
                var subscription = new ClubSubscription
                {
                    UserId = userId,
                    ClubId = clubId,
                    PlanId = request.PlanId,
                    StartDate = request.StartDate.ToUniversalTime(),
                    EndDate = request.EndDate.ToUniversalTime(),
                    Price = calculatedPrice,
                    Status = SubscriptionStatus.Active, // Payment already deducted
                    CreatedAt = DateTime.UtcNow
                };

                _context.ClubSubscriptions.Add(subscription);
                await _context.SaveChangesAsync(ct);

                // Create payment record
                var payment = new SubscriptionPayment
                {
                    ClubSubscriptionId = subscription.Id,
                    Amount = calculatedPrice,
                    Status = PaymentStatus.Paid,
                    PaidAt = DateTime.UtcNow,
                    TransactionId = $"SUB-{subscription.Id}" // Placeholder transaction ID
                };

                _context.SubscriptionPayments.Add(payment);
                await _context.SaveChangesAsync(ct);

                await transaction.CommitAsync(ct);

                // Map to response
                var response = await MapToResponseAsync(subscription, ct);
                return Result.Success(response);
            }
            catch
            {
                await transaction.RollbackAsync(ct);

                // Refund the wallet deduction on failure
                await _walletService.CreditAsync(
                    userId, calculatedPrice, "Subscription.Subscribe rollback", ct);

                throw;
            }
        }

        /// <summary>
        /// Gets the active subscription for a user at a specific club.
        /// </summary>
        /// <remarks>
        /// Returns only subscriptions with Status = Active.
        /// Uses AsNoTracking() for read-only query and proper indexing.
        /// </remarks>
        /// <param name="userId">The user's ID.</param>
        /// <param name="clubId">The club's ID.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// Success with subscription if found and active.
        /// Failure codes:
        /// - Validation.UserId (400) - userId null/empty
        /// - Validation.ClubId (400) - clubId null/empty
        /// - Subscription.NotFound (404) - no active subscription found
        /// </returns>
        public async Task<Result<ClubSubscriptionResponse>> GetActiveSubscriptionAsync(
            string userId, string clubId, CancellationToken ct = default)
        {
            var idValidation = ValidateIds(userId, clubId);
            if (idValidation.IsFailure)
                return Result.Failure<ClubSubscriptionResponse>(idValidation.Error);

            var subscription = await _context.ClubSubscriptions
                .Where(s => s.ClubId == clubId && !s.IsDeleted &&
                            s.StartDate <= now && s.EndDate >= now)
                .Select(s => new ClubSubscriptionResponse(
                    s.Id,
                    new ClubSummary(s.Club.Id, s.Club.Name, s.Club.LogoUrl, s.Club.City, s.Club.Governorate),
                    new SubscriptionPlanSummary(s.Plan.Id, s.Plan.Name, s.Plan.MonthlyPrice, s.Plan.MaxCourts),
                    s.StartDate,
                    s.EndDate,
                    true,
                    s.Payments.Count,
                    s.Payments
                        .OrderByDescending(p => p.PaidAt)
                        .ThenByDescending(p => p.Id)
                        .Select(p => new SubscriptionPaymentSummary(
                            p.Id, p.Amount, (PaymentStatusDto)p.Status, p.TransactionId, p.PaidAt))
                        .FirstOrDefault()
                ))
                .AsNoTracking()
                .Include(s => s.Plan)
                .Include(s => s.Club)
                .Include(s => s.Payments)
                .FirstOrDefaultAsync(s =>
                    s.UserId == userId &&
                    s.ClubId == clubId &&
                    s.Status == SubscriptionStatus.Active &&
                    !s.IsDeleted, ct);

            if (subscription is null)
                return Result.Failure<ClubSubscriptionResponse>(SubscriptionErrors.SubscriptionNotFound);

            return Result.Success(subscription);
        }
        catch (Exception ex)
        {
                return Result.Failure<ClubSubscriptionResponse>(new Error(
                    "Subscription.NotFound", "No active subscription found for this user and club", 404));
        }

            var response = MapToResponse(subscription);
            return Result.Success(response);
    }

        /// <summary>
        /// Gets paginated subscription history for a user at a specific club.
        /// </summary>
        /// <remarks>
        /// Supports filtering by status and pagination.
        /// Validates page number >= 1 and page size <= 50.
        /// Sorted by CreatedAt descending (most recent first).
        /// </remarks>
        /// <param name="userId">The user's ID.</param>
        /// <param name="clubId">The club's ID.</param>
        /// <param name="filters">Pagination and optional status filter.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// Success with paginated subscription list.
        /// Failure codes:
        /// - Validation.UserId (400) - userId null/empty
        /// - Validation.ClubId (400) - clubId null/empty
        /// - Validation.Filters (400) - invalid pagination parameters
        /// </returns>
    public async Task<Result<PaginatedList<ClubSubscriptionResponse>>> GetSubscriptionHistoryAsync(
        string userId, string clubId, RequestFilters filters, CancellationToken ct = default)
    {
            var idValidation = ValidateIds(userId, clubId);
            if (idValidation.IsFailure)
                return Result.Failure<PaginatedList<ClubSubscriptionResponse>>(idValidation.Error);

            // Validate filters
            if (filters.PageNumber < 1)
        {
                return Result.Failure<PaginatedList<ClubSubscriptionResponse>>(new Error(
                    "Validation.Filters", "PageNumber must be >= 1", 400));
            }

            // PageSize max is validated in RequestFilters itself (50), but double-check
            if (filters.PageSize < 1 || filters.PageSize > 50)
            {
                return Result.Failure<PaginatedList<ClubSubscriptionResponse>>(new Error(
                    "Validation.Filters", "PageSize must be between 1 and 50", 400));
            }

            var query = _context.ClubSubscriptions
                .AsNoTracking()
                .Where(s =>
                    s.UserId == userId &&
                    s.ClubId == clubId &&
                    !s.IsDeleted)
                .Include(s => s.Plan)
                .Include(s => s.Club)
                .Include(s => s.Payments)
                .OrderByDescending(s => s.CreatedAt);

            var paginatedList = await PaginatedList<ClubSubscription>.CreateAsync(
                query, filters.PageNumber, filters.PageSize, ct);

            var responses = paginatedList.Items.Select(MapToResponse).ToList();

            //var result = new PaginatedList<ClubSubscriptionResponse>
            //{
            //    Items = responses,
            //    PageNumber = paginatedList.PageNumber,
            //    PageSize = paginatedList.PageSize,
            //    TotalCount = paginatedList.TotalCount,
            //    TotalPages = paginatedList.TotalPages
            //};

            var result = await _context.ClubSubscriptions
            // 1. ضع الفلاتر الخاصة بك هنا
            .Where(cs => cs.ClubId == clubId)
            // 2. تحويل البيانات (Mapping) مباشرة في الاستعلام
            .ProjectToType<ClubSubscriptionResponse>()
            .AsNoTracking()
            // 3. تطبيق الـ Pagination في النهاية
            .ToPaginatedListAsync(filters, ct);

            return Result.Success(result);
        }

        /// <summary>
        /// Renews an expired or cancelled subscription for the user.
        /// </summary>
        /// <remarks>
        /// Business rules:
        /// - Can only renew if status is Cancelled or Expired
        /// - Cannot renew if club is inactive
        /// - Payment is deducted from wallet
        /// - New subscription record is created with updated dates
        /// </remarks>
        /// <param name="userId">The user's ID.</param>
        /// <param name="clubId">The club's ID.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// Success with new ClubSubscriptionResponse if renewed.
        /// Failure codes:
        /// - Validation.UserId (400) - userId null/empty
        /// - Validation.ClubId (400) - clubId null/empty
        /// - Subscription.NotFound (404) - no subscription found
        /// - Subscription.Forbidden (403) - subscription doesn't belong to user
        /// - Subscription.CannotRenew (409) - subscription is not in Cancelled or Expired state
        /// - Club.Inactive (403) - club is no longer active
        /// - Plan.Inactive (403) - plan is no longer active
        /// - Wallet.InsufficientBalance (402) - insufficient funds
        /// </returns>
        public async Task<Result<ClubSubscriptionResponse>> RenewSubscriptionAsync(
            string userId, string clubId, CancellationToken ct = default)
        {
            var idValidation = ValidateIds(userId, clubId);
            if (idValidation.IsFailure)
                return Result.Failure<ClubSubscriptionResponse>(idValidation.Error);

            // Load existing subscription
            var existingSubscription = await _context.ClubSubscriptions
                .Include(s => s.Plan)
                .Include(s => s.Club)
                .FirstOrDefaultAsync(s =>
                    s.UserId == userId &&
                    s.ClubId == clubId &&
                    !s.IsDeleted, ct);

            if (existingSubscription is null)
            {
                return Result.Failure<ClubSubscriptionResponse>(new Error(
                    "Subscription.NotFound", "Subscription not found", 404));
        }

            // Ownership check
            if (existingSubscription.UserId != userId)
            {
                return Result.Failure<ClubSubscriptionResponse>(new Error(
                    "Subscription.Forbidden", "Not authorized to renew this subscription", 403));
    }

            // Can only renew if Cancelled or Expired
            if (existingSubscription.Status != SubscriptionStatus.Cancelled &&
                existingSubscription.Status != SubscriptionStatus.Expired)
        {
                return Result.Failure<ClubSubscriptionResponse>(new Error(
                    "Subscription.CannotRenew",
                    $"Cannot renew subscription in {existingSubscription.Status} state. Only Cancelled or Expired subscriptions can be renewed.",
                    409));
            }

            // Verify club is still active
            var club = await _context.Clubs
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == clubId && !c.IsDeleted, ct);

            if (club is null || !club.IsActive)
            {
                return Result.Failure<ClubSubscriptionResponse>(new Error(
                    "Club.Inactive", "Cannot renew: club is no longer active", 403));
            }

            // Verify plan is still active
            var plan = existingSubscription.Plan;
            if (!plan.IsActive)
            {
                return Result.Failure<ClubSubscriptionResponse>(new Error(
                    "Plan.Inactive", "Cannot renew: plan is no longer active", 403));
            }

            // Calculate new subscription period (same duration as original, starting now)
            var originalDurationDays = (existingSubscription.EndDate.Date - existingSubscription.StartDate.Date).Days + 1;
            var newStartDate = DateTime.UtcNow;
            var newEndDate = newStartDate.AddDays(originalDurationDays - 1); // -1 to match original duration

            // Calculate price
            var durationDays = (newEndDate.Date - newStartDate.Date).Days + 1;
            var dailyRate = plan.MonthlyPrice / 30m;
            var calculatedPrice = dailyRate * durationDays;

            // Deduct payment
            var deductResult = await _walletService.DeductAsync(
                userId, calculatedPrice, $"Subscription.Renew {club.Name ?? clubId}", ct);

            if (deductResult.IsFailure)
            {
                return Result.Failure<ClubSubscriptionResponse>(deductResult.Error);
            }

            // Create new subscription record
            using var transaction = await _context.Database.BeginTransactionAsync(ct);
            try
            {
                var newSubscription = new ClubSubscription
                {
                    UserId = userId,
                    ClubId = clubId,
                    PlanId = plan.Id,
                    StartDate = newStartDate,
                    EndDate = newEndDate,
                    Price = calculatedPrice,
                    Status = SubscriptionStatus.Active,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ClubSubscriptions.Add(newSubscription);
                await _context.SaveChangesAsync(ct);

                // Create payment record
                var payment = new SubscriptionPayment
            {
                    ClubSubscriptionId = newSubscription.Id,
                    Amount = calculatedPrice,
                    Status = PaymentStatus.Paid,
                    PaidAt = DateTime.UtcNow,
                    TransactionId = $"REN-{newSubscription.Id}"
            };

                _context.SubscriptionPayments.Add(payment);
            await _context.SaveChangesAsync(ct);

                await transaction.CommitAsync(ct);

                var response = await MapToResponseAsync(newSubscription, ct);
            return Result.Success(response);
        }
            catch
        {
                await transaction.RollbackAsync(ct);
                await _walletService.CreditAsync(
                    userId, calculatedPrice, "Subscription.Renew rollback", ct);
                throw;
        }
    }

        /// <summary>
        /// Cancels an active subscription and processes refund.
        /// </summary>
        /// <remarks>
        /// Business logic:
        /// - Can cancel Active or PendingPayment subscriptions
        /// - Cannot cancel already Cancelled or Expired subscriptions
        /// - Calculates refund based on unused days (with non-refundable period)
        /// - Refund = (DailyRate × RemainingDays) if outside non-refundable window
        /// - Credits refund to user's wallet
        /// </remarks>
        /// <param name="userId">The user's ID.</param>
        /// <param name="clubId">The club's ID.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// Success if cancellation processed (with or without refund).
        /// Failure codes:
        /// - Validation.UserId (400) - userId null/empty
        /// - Validation.ClubId (400) - clubId null/empty
        /// - Subscription.NotFound (404) - no subscription found
        /// - Subscription.Forbidden (403) - subscription doesn't belong to user
        /// - Subscription.AlreadyCancelled (409) - already cancelled
        /// - Subscription.Expired (409) - subscription already expired
        /// </returns>
        public async Task<Result> CancelSubscriptionAsync(
        string userId, string clubId, CancellationToken ct = default)
    {
            var idValidation = ValidateIds(userId, clubId);
            if (idValidation.IsFailure)
                return idValidation;

            // Load subscription with club for context
            var subscription = await _context.ClubSubscriptions
                .Include(s => s.Club)
                .FirstOrDefaultAsync(s =>
                    s.UserId == userId &&
                    s.ClubId == clubId &&
                    !s.IsDeleted, ct);

            if (subscription is null)
            {
                return Result.Failure(new Error(
                    "Subscription.NotFound", "Subscription not found", 404));
            }

            // Ownership check (defensive re-check)
            if (subscription.UserId != userId)
        {
                return Result.Failure(new Error(
                    "Subscription.Forbidden", "Not authorized to cancel this subscription", 403));
            }

            // Status checks
            if (subscription.Status == SubscriptionStatus.Cancelled)
            {
                return Result.Failure(new Error(
                    "Subscription.AlreadyCancelled", "Subscription is already cancelled", 409));
            }

            if (subscription.Status == SubscriptionStatus.Expired)
            {
                return Result.Failure(new Error(
                    "Subscription.Expired", "Cannot cancel an already expired subscription", 409));
            }

            // Calculate refund
            var refundAmount = CalculateRefund(subscription);

            // Update subscription to cancelled state
            subscription.Status = SubscriptionStatus.Cancelled;
            subscription.CancelledAt = DateTime.UtcNow;
            subscription.RefundAmount = refundAmount;

            _context.ClubSubscriptions.Update(subscription);
            await _context.SaveChangesAsync(ct);

            // Process refund if applicable
            if (refundAmount > MinimumRefundAmount)
            {
                await _walletService.CreditAsync(
                    userId,
                    refundAmount,
                    $"Subscription.Cancel refund from {subscription.Club?.Name ?? clubId}",
                    ct);
            }

            return Result.Success();
        }

        #region Private Helper Methods

        /// <summary>
        /// Validates that userId and clubId are not null/empty/whitespace.
        /// </summary>
        private static Result ValidateIds(string userId, string clubId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result.Failure(new Error("Validation.UserId", "UserId cannot be null or empty", 400));
        }

            if (string.IsNullOrWhiteSpace(clubId))
        {
                return Result.Failure(new Error("Validation.ClubId", "ClubId cannot be null or empty", 400));
        }

            return Result.Success();
    }

        /// <summary>
        /// Validates that the date range is valid and within acceptable bounds.
        /// </summary>
        private static Result ValidateDateRange(DateTime startDate, DateTime endDate)
    {
            var utcStart = startDate.ToUniversalTime();
            var utcEnd = endDate.ToUniversalTime();

            if (utcEnd <= utcStart)
        {
                return Result.Failure(new Error(
                    "Validation.DateRange", "EndDate must be after StartDate", 400));
            }

            var durationDays = (utcEnd.Date - utcStart.Date).Days + 1;

            if (durationDays < MinSubscriptionDays)
            {
                return Result.Failure(new Error(
                    "Validation.DateRange",
                    $"Subscription duration must be at least {MinSubscriptionDays} day(s)",
                    400));
            }

            if (durationDays > MaxSubscriptionDays)
            {
                return Result.Failure(new Error(
                    "Validation.DateRange",
                    $"Subscription duration cannot exceed {MaxSubscriptionDays} days",
                    400));
            }

            return Result.Success();
        }

        /// <summary>
        /// Calculates refund amount based on unused days.
        /// Formula: DailyRate × RemainingDays, with non-refundable period applied.
        /// </summary>
        private decimal CalculateRefund(ClubSubscription subscription)
        {
            var now = DateTime.UtcNow;

            // If subscription already expired, no refund
            if (now >= subscription.EndDate)
            {
                return 0m;
            }

            // If within non-refundable period (last N days), no refund
            var daysUntilExpiration = (subscription.EndDate - now).Days;
            if (daysUntilExpiration <= NonRefundablePeriodDays)
        {
                return 0m;
        }

            // Calculate daily rate
            var totalDays = (subscription.EndDate.Date - subscription.StartDate.Date).Days + 1;
            var dailyRate = subscription.Price / totalDays;

            // Calculate remaining days (outside non-refundable window)
            var remainingDaysBeforeNonRefundable = daysUntilExpiration - NonRefundablePeriodDays;
            var refund = dailyRate * remainingDaysBeforeNonRefundable;

            return Math.Max(0, Math.Round(refund, 2)); // Floor at 0, round to 2 decimals
    }

        /// <summary>
        /// Maps a ClubSubscription entity to ClubSubscriptionResponse DTO (synchronous).
        /// </summary>
        private ClubSubscriptionResponse MapToResponse(ClubSubscription subscription)
    {
            var lastPayment = subscription.Payments
                .OrderByDescending(p => p.PaidAt)
                .FirstOrDefault();

            var clubSummary = new ClubSummary(
                subscription.Club.Id,
                subscription.Club.Name ?? string.Empty,
                subscription.Club.LogoUrl,
                subscription.Club.City ?? string.Empty,
                subscription.Club.Governorate ?? string.Empty
                );

            var planSummary = new SubscriptionPlanSummary(
                subscription.Plan.Id,
                subscription.Plan.Name ?? string.Empty,
                subscription.Plan.MonthlyPrice,
                subscription.Plan.MaxCourts);

            var lastPaymentSummary = lastPayment is not null
                ? new SubscriptionPaymentSummary(
                    lastPayment.Id,
                    lastPayment.Amount,
                    (PaymentStatusDto)lastPayment.Status,
                    lastPayment.TransactionId,
                    lastPayment.PaidAt)
                : null;

            return new ClubSubscriptionResponse(
                subscription.Id,
                clubSummary,
                planSummary,
                subscription.StartDate,
                subscription.EndDate,
                subscription.Status == SubscriptionStatus.Active,
                subscription.Payments.Count,
                lastPaymentSummary);
        }

        /// <summary>
        /// Maps a ClubSubscription entity to ClubSubscriptionResponse DTO (asynchronous).
        /// Used after creating a new subscription to ensure full eager-loaded data.
        /// </summary>
        private async Task<ClubSubscriptionResponse> MapToResponseAsync(
            ClubSubscription subscription, CancellationToken ct = default)
        {
            // Reload with all navigation properties to ensure they're loaded
            var loaded = await _context.ClubSubscriptions
            .AsNoTracking()
                .Include(s => s.Club)
                .Include(s => s.Plan)
                .Include(s => s.Payments)
                .FirstOrDefaultAsync(s => s.Id == subscription.Id, ct);

            if (loaded is null)
            {
                // Fallback (shouldn't happen, but defensive)
                return MapToResponse(subscription);
            }

            return MapToResponse(loaded);
        }

        #endregion
    }
}
