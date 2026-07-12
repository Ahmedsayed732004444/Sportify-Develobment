using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sportiva.Contracts.Common;
using Sportiva.Contracts.Shared.Enums;
using Sportiva.Contracts.Shared.Summaries;
using Sportiva.Contracts.Subscriptions;
using Sportiva.Entities;
using Sportiva.Errors;
using Sportiva.Extensions;
using Sportiva.Persistence;

namespace Sportiva.Services;

public class ClubSubscriptionService(
    ApplicationDbContext context,
    ILogger<ClubSubscriptionService> logger) : IClubSubscriptionService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<ClubSubscriptionService> _logger = logger;

    private static readonly string[] AllowedSortColumns = ["StartDate", "EndDate"];

    public async Task<Result<ClubSubscriptionResponse>> GetActiveSubscriptionAsync(
        string userId, string clubId, CancellationToken ct = default)
    {
        try
        {
            var authorize = await AuthorizeClubOwnerAsync(userId, clubId, ct);
            if (authorize.IsFailure)
                return Result.Failure<ClubSubscriptionResponse>(authorize.Error);

            var now = DateTime.UtcNow;

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
                .FirstOrDefaultAsync(ct);

            if (subscription is null)
                return Result.Failure<ClubSubscriptionResponse>(SubscriptionErrors.SubscriptionNotFound);

            return Result.Success(subscription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving active subscription for club {ClubId}.", clubId);
            return Result.Failure<ClubSubscriptionResponse>(SubscriptionErrors.Error);
        }
    }

    public async Task<Result<PaginatedList<ClubSubscriptionResponse>>> GetSubscriptionHistoryAsync(
        string userId, string clubId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var authorize = await AuthorizeClubOwnerAsync(userId, clubId, ct);
            if (authorize.IsFailure)
                return Result.Failure<PaginatedList<ClubSubscriptionResponse>>(authorize.Error);

            var now = DateTime.UtcNow;

            var query = _context.ClubSubscriptions
                .Where(s => s.ClubId == clubId && !s.IsDeleted)
                .ApplyFilters(filters, allowedSortColumns: AllowedSortColumns)
                .Select(s => new ClubSubscriptionResponse(
                    s.Id,
                    new ClubSummary(s.Club.Id, s.Club.Name, s.Club.LogoUrl, s.Club.City, s.Club.Governorate),
                    new SubscriptionPlanSummary(s.Plan.Id, s.Plan.Name, s.Plan.MonthlyPrice, s.Plan.MaxCourts),
                    s.StartDate,
                    s.EndDate,
                    s.StartDate <= now && s.EndDate >= now,
                    s.Payments.Count,
                    s.Payments
                        .OrderByDescending(p => p.PaidAt)
                        .ThenByDescending(p => p.Id)
                        .Select(p => new SubscriptionPaymentSummary(
                            p.Id, p.Amount, (PaymentStatusDto)p.Status, p.TransactionId, p.PaidAt))
                        .FirstOrDefault()
                ));

            var result = await query.AsNoTracking().ToPaginatedListAsync(filters, ct);
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving subscription history for club {ClubId}.", clubId);
            return Result.Failure<PaginatedList<ClubSubscriptionResponse>>(SubscriptionErrors.Error);
        }
    }

    public async Task<Result<ClubSubscriptionResponse>> SubscribeAsync(
        string userId, string clubId, CreateClubSubscriptionRequest request, CancellationToken ct = default)
    {
        try
        {
            var authorize = await AuthorizeClubOwnerAsync(userId, clubId, ct);
            if (authorize.IsFailure)
                return Result.Failure<ClubSubscriptionResponse>(authorize.Error);

            if (string.IsNullOrWhiteSpace(request.PlanId))
                return Result.Failure<ClubSubscriptionResponse>(SubscriptionErrors.PlanNotFound);

            if (request.StartDate == default || request.EndDate == default)
                return Result.Failure<ClubSubscriptionResponse>(SubscriptionErrors.InvalidDateRange);

            if (request.EndDate <= request.StartDate)
                return Result.Failure<ClubSubscriptionResponse>(SubscriptionErrors.InvalidDateRange);

            var plan = await _context.SubscriptionPlans.AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.PlanId, ct);

            if (plan is null)
                return Result.Failure<ClubSubscriptionResponse>(SubscriptionErrors.PlanNotFound);

            if (!plan.IsActive)
                return Result.Failure<ClubSubscriptionResponse>(SubscriptionErrors.PlanInactive);

            var now = DateTime.UtcNow;
            var hasActive = await _context.ClubSubscriptions.AnyAsync(
                s => s.ClubId == clubId && !s.IsDeleted && s.StartDate <= now && s.EndDate >= now, ct);

            if (hasActive)
                return Result.Failure<ClubSubscriptionResponse>(SubscriptionErrors.ActiveSubscriptionExists);

            var subscription = new ClubSubscription
            {
                ClubId = clubId,
                PlanId = request.PlanId,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };

            await _context.ClubSubscriptions.AddAsync(subscription, ct);
            await _context.SaveChangesAsync(ct);

            var response = await _context.ClubSubscriptions
                .Where(s => s.Id == subscription.Id)
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
                .FirstAsync(ct);

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while subscribing club {ClubId}.", clubId);
            return Result.Failure<ClubSubscriptionResponse>(SubscriptionErrors.Error);
        }
    }

    public async Task<Result<ClubSubscriptionResponse>> RenewSubscriptionAsync(
        string userId, string clubId, CancellationToken ct = default)
    {
        try
        {
            var authorize = await AuthorizeClubOwnerAsync(userId, clubId, ct);
            if (authorize.IsFailure)
                return Result.Failure<ClubSubscriptionResponse>(authorize.Error);

            var latest = await _context.ClubSubscriptions
                .Where(s => s.ClubId == clubId && !s.IsDeleted)
                .OrderByDescending(s => s.EndDate)
                .FirstOrDefaultAsync(ct);

            if (latest is null)
                return Result.Failure<ClubSubscriptionResponse>(SubscriptionErrors.NoSubscriptionToRenew);

            var plan = await _context.SubscriptionPlans.AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == latest.PlanId, ct);

            if (plan is null || !plan.IsActive)
                return Result.Failure<ClubSubscriptionResponse>(SubscriptionErrors.PlanInactive);

            var now = DateTime.UtcNow;
            var newStart = latest.EndDate > now ? latest.EndDate : now;
            var newEnd = newStart.AddMonths(1);

            var subscription = new ClubSubscription
            {
                ClubId = clubId,
                PlanId = latest.PlanId,
                StartDate = newStart,
                EndDate = newEnd
            };

            await _context.ClubSubscriptions.AddAsync(subscription, ct);
            await _context.SaveChangesAsync(ct);

            var response = await _context.ClubSubscriptions
                .Where(s => s.Id == subscription.Id)
                .Select(s => new ClubSubscriptionResponse(
                    s.Id,
                    new ClubSummary(s.Club.Id, s.Club.Name, s.Club.LogoUrl, s.Club.City, s.Club.Governorate),
                    new SubscriptionPlanSummary(s.Plan.Id, s.Plan.Name, s.Plan.MonthlyPrice, s.Plan.MaxCourts),
                    s.StartDate,
                    s.EndDate,
                    s.StartDate <= now && s.EndDate >= now,
                    s.Payments.Count,
                    s.Payments
                        .OrderByDescending(p => p.PaidAt)
                        .ThenByDescending(p => p.Id)
                        .Select(p => new SubscriptionPaymentSummary(
                            p.Id, p.Amount, (PaymentStatusDto)p.Status, p.TransactionId, p.PaidAt))
                        .FirstOrDefault()
                ))
                .AsNoTracking()
                .FirstAsync(ct);

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while renewing subscription for club {ClubId}.", clubId);
            return Result.Failure<ClubSubscriptionResponse>(SubscriptionErrors.Error);
        }
    }

    public async Task<Result> CancelSubscriptionAsync(
        string userId, string clubId, CancellationToken ct = default)
    {
        try
        {
            var authorize = await AuthorizeClubOwnerAsync(userId, clubId, ct);
            if (authorize.IsFailure)
                return authorize;

            var now = DateTime.UtcNow;
            var subscription = await _context.ClubSubscriptions.FirstOrDefaultAsync(
                s => s.ClubId == clubId && !s.IsDeleted && s.StartDate <= now && s.EndDate >= now, ct);

            if (subscription is null)
                return Result.Failure(SubscriptionErrors.SubscriptionNotFound);

            subscription.EndDate = now;
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while cancelling subscription for club {ClubId}.", clubId);
            return Result.Failure(SubscriptionErrors.Error);
        }
    }

    private async Task<Result> AuthorizeClubOwnerAsync(string userId, string clubId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(clubId))
            return Result.Failure(ClubErrors.ClubNotFound);

        var club = await _context.Clubs
            .AsNoTracking()
            .Where(c => c.Id == clubId)
            .Select(c => new { c.OwnerId })
            .FirstOrDefaultAsync(ct);

        if (club is null)
            return Result.Failure(ClubErrors.ClubNotFound);

        if (club.OwnerId != userId)
            return Result.Failure(ClubErrors.Unauthorized);

        return Result.Success();
    }
}
