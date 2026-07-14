using Microsoft.EntityFrameworkCore;
using Sportiva.Abstractions;
using Sportiva.Contracts.Common;
using Sportiva.Contracts.Shared.Enums;
using Sportiva.Contracts.Shared.Summaries;
using Sportiva.Contracts.Subscriptions;
using Sportiva.Entities;
using Sportiva.Enums;
using Sportiva.Extensions;
using Sportiva.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sportiva.Services.Implementation
{
    /// <summary>
    /// Service for managing platform subscription plans for clubs (owner-centric).
    /// </summary>
    public class ClubSubscriptionService : IClubSubscriptionService
    {
        private readonly ApplicationDbContext _context;

        // Named constants for business rules
        private static readonly string[] SubscriptionSortColumns = ["CreatedAt", "StartDate", "EndDate", "Status"];

        public ClubSubscriptionService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the active subscription for the specified club.
        /// </summary>
        public async Task<Result<ClubSubscriptionResponse>> GetActiveSubscriptionAsync(
            string userId, string clubId, CancellationToken ct = default)
        {
            var clubResult = await LoadClubWithOwnershipCheckAsync(userId, clubId, ct);
            if (clubResult.IsFailure)
            {
                return Result.Failure<ClubSubscriptionResponse>(clubResult.Error);
            }

            var subscription = await _context.ClubSubscriptions
                .AsNoTracking()
                .Where(s => s.ClubId == clubId && s.Status == SubscriptionStatus.Active && !s.IsDeleted)
                .ProjectToType<ClubSubscriptionResponse>()
                .FirstOrDefaultAsync(ct);

            if (subscription is null)
            {
                return Result.Failure<ClubSubscriptionResponse>(new Error(
                    "Subscription.NotFound", "Active subscription not found for this club", 404));
            }

            return Result.Success(subscription);
        }

        /// <summary>
        /// Gets paginated subscription history for the specified club.
        /// </summary>
        public async Task<Result<PaginatedList<ClubSubscriptionResponse>>> GetSubscriptionHistoryAsync(
            string userId, string clubId, RequestFilters filters, CancellationToken ct = default)
        {
            var clubResult = await LoadClubWithOwnershipCheckAsync(userId, clubId, ct);
            if (clubResult.IsFailure)
            {
                return Result.Failure<PaginatedList<ClubSubscriptionResponse>>(clubResult.Error);
            }

            // Validate filters
            if (filters.PageNumber < 1)
            {
                return Result.Failure<PaginatedList<ClubSubscriptionResponse>>(new Error(
                    "Validation.Filters", "PageNumber must be >= 1", 400));
            }

            if (filters.PageSize < 1 || filters.PageSize > 50)
            {
                return Result.Failure<PaginatedList<ClubSubscriptionResponse>>(new Error(
                    "Validation.Filters", "PageSize must be between 1 and 50", 400));
            }

            var result = await _context.ClubSubscriptions
                .AsNoTracking()
                .Where(s => s.ClubId == clubId && !s.IsDeleted)
                .ApplyFilters(
                    filters,
                    searchPredicate: s => s.Plan.Name.Contains(filters.SearchValue!),
                    allowedSortColumns: SubscriptionSortColumns)
                .ProjectToType<ClubSubscriptionResponse>()
                .AsNoTracking()
                .ToPaginatedListAsync(filters, ct);

            return Result.Success(result);
        }

        /// <summary>
        /// Subscribes a club to a platform subscription plan.
        /// </summary>
        public async Task<Result<ClubSubscriptionResponse>> SubscribeAsync(
            string userId, string clubId, CreateClubSubscriptionRequest request, CancellationToken ct = default)
        {
            var clubResult = await LoadClubWithOwnershipCheckAsync(userId, clubId, ct);
            if (clubResult.IsFailure)
            {
                return Result.Failure<ClubSubscriptionResponse>(clubResult.Error);
            }

            if (!clubResult.Value.IsActive)
            {
                return Result.Failure<ClubSubscriptionResponse>(new Error(
                    "Club.Inactive", "Cannot subscribe to an inactive club", 403));
            }

            if (string.IsNullOrWhiteSpace(request.PlanId))
            {
                return Result.Failure<ClubSubscriptionResponse>(new Error(
                    "Validation.PlanId", "PlanId cannot be null or empty", 400));
            }

            // Load subscription plan
            var plan = await _context.SubscriptionPlans
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.PlanId && !p.IsDeleted && p.IsActive, ct);

            if (plan is null)
            {
                return Result.Failure<ClubSubscriptionResponse>(new Error(
                    "SubscriptionPlan.NotFound", "Subscription plan not found", 404));
            }

            // Verify no active subscription exists
            var existingActive = await _context.ClubSubscriptions
                .AsNoTracking()
                .AnyAsync(s => s.ClubId == clubId && s.Status == SubscriptionStatus.Active && !s.IsDeleted, ct);

            if (existingActive)
            {
                return Result.Failure<ClubSubscriptionResponse>(new Error(
                    "Subscription.Conflict", "Club already has an active subscription", 409));
            }

            // Derive price and duration
            var price = plan.MonthlyPrice;
            var startDate = request.StartDate.ToUniversalTime();
            var endDate = startDate.AddMonths(1); // Standard 1 month platform plan

            using var transaction = await _context.Database.BeginTransactionAsync(ct);
            try
            {
                var subscription = new ClubSubscription
                {
                    UserId = userId,
                    ClubId = clubId,
                    PlanId = plan.Id,
                    StartDate = startDate,
                    EndDate = endDate,
                    Price = price,
                    Status = SubscriptionStatus.PendingPayment,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ClubSubscriptions.Add(subscription);
                await _context.SaveChangesAsync(ct);

                var payment = new SubscriptionPayment
                {
                    ClubSubscriptionId = subscription.Id,
                    Amount = price,
                    Status = PaymentStatus.Pending,
                    PaidAt = null,
                    TransactionId = null
                };

                _context.SubscriptionPayments.Add(payment);
                await _context.SaveChangesAsync(ct);

                await transaction.CommitAsync(ct);

                // Populate navigation properties for in-memory mapping
                subscription.Club = clubResult.Value;
                subscription.Plan = plan;
                subscription.Payments = new List<SubscriptionPayment> { payment };

                var response = subscription.Adapt<ClubSubscriptionResponse>();
                return Result.Success(response);
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }

        /// <summary>
        /// Renews an expired or cancelled subscription for the club.
        /// </summary>
        public async Task<Result<ClubSubscriptionResponse>> RenewSubscriptionAsync(
            string userId, string clubId, CancellationToken ct = default)
        {
            var clubResult = await LoadClubWithOwnershipCheckAsync(userId, clubId, ct);
            if (clubResult.IsFailure)
            {
                return Result.Failure<ClubSubscriptionResponse>(clubResult.Error);
            }

            if (!clubResult.Value.IsActive)
            {
                return Result.Failure<ClubSubscriptionResponse>(new Error(
                    "Club.Inactive", "Cannot renew: club is no longer active", 403));
            }

            // Load existing subscription to renew
            var existingSubscription = await _context.ClubSubscriptions
                .Include(s => s.Plan)
                .Include(s => s.Club)
                .Where(s => s.ClubId == clubId && !s.IsDeleted)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync(ct);

            if (existingSubscription is null)
            {
                return Result.Failure<ClubSubscriptionResponse>(new Error(
                    "Subscription.NotFound", "Subscription not found", 404));
            }

            if (existingSubscription.Status != SubscriptionStatus.Cancelled &&
                existingSubscription.Status != SubscriptionStatus.Expired)
            {
                return Result.Failure<ClubSubscriptionResponse>(new Error(
                    "Subscription.CannotRenew",
                    $"Cannot renew subscription in {existingSubscription.Status} state. Only Cancelled or Expired subscriptions can be renewed.",
                    409));
            }

            var plan = existingSubscription.Plan;
            if (plan is null || !plan.IsActive || plan.IsDeleted)
            {
                return Result.Failure<ClubSubscriptionResponse>(new Error(
                    "Plan.Inactive", "Cannot renew: subscription plan is no longer active", 403));
            }

            // Derive price and duration
            var price = plan.MonthlyPrice;
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddMonths(1);

            using var transaction = await _context.Database.BeginTransactionAsync(ct);
            try
            {
                var newSubscription = new ClubSubscription
                {
                    UserId = userId,
                    ClubId = clubId,
                    PlanId = plan.Id,
                    StartDate = startDate,
                    EndDate = endDate,
                    Price = price,
                    Status = SubscriptionStatus.PendingPayment,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ClubSubscriptions.Add(newSubscription);
                await _context.SaveChangesAsync(ct);

                var payment = new SubscriptionPayment
                {
                    ClubSubscriptionId = newSubscription.Id,
                    Amount = price,
                    Status = PaymentStatus.Pending,
                    PaidAt = null,
                    TransactionId = null
                };

                _context.SubscriptionPayments.Add(payment);
                await _context.SaveChangesAsync(ct);

                await transaction.CommitAsync(ct);

                // Populate navigation properties for mapping
                newSubscription.Club = clubResult.Value;
                newSubscription.Plan = plan;
                newSubscription.Payments = new List<SubscriptionPayment> { payment };

                var response = newSubscription.Adapt<ClubSubscriptionResponse>();
                return Result.Success(response);
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }

        /// <summary>
        /// Cancels the active subscription for the club.
        /// </summary>
        public async Task<Result> CancelSubscriptionAsync(
            string userId, string clubId, CancellationToken ct = default)
        {
            var clubResult = await LoadClubWithOwnershipCheckAsync(userId, clubId, ct);
            if (clubResult.IsFailure)
            {
                return Result.Failure(clubResult.Error);
            }

            var subscription = await _context.ClubSubscriptions
                .Include(s => s.Club)
                .FirstOrDefaultAsync(s => s.ClubId == clubId && s.Status == SubscriptionStatus.Active && !s.IsDeleted, ct);

            if (subscription is null)
            {
                return Result.Failure(new Error(
                    "Subscription.NotFound", "Active subscription not found for this club", 404));
            }

            subscription.Status = SubscriptionStatus.Cancelled;
            subscription.CancelledAt = DateTime.UtcNow;

            _context.ClubSubscriptions.Update(subscription);
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }

        #region Private Helper Methods

        /// <summary>
        /// Loads the club and performs user ownership validation.
        /// </summary>
        private async Task<Result<Club>> LoadClubWithOwnershipCheckAsync(
            string userId, string clubId, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result.Failure<Club>(new Error("Validation.UserId", "UserId cannot be null or empty", 400));
            }

            if (string.IsNullOrWhiteSpace(clubId))
            {
                return Result.Failure<Club>(new Error("Validation.ClubId", "ClubId cannot be null or empty", 400));
            }

            var club = await _context.Clubs
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == clubId && !c.IsDeleted, ct);

            if (club is null)
            {
                return Result.Failure<Club>(new Error("Club.NotFound", "Club not found", 404));
            }

            if (club.OwnerId != userId)
            {
                return Result.Failure<Club>(new Error("Club.Forbidden", "Not authorized to access this club", 403));
            }

            return Result.Success(club);
        }

        #endregion
    }
}
