using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sportiva.Contracts.Subscriptions;
using Sportiva.Entities;
using Sportiva.Errors;
using Sportiva.Persistence;

namespace Sportiva.Services;

public class SubscriptionPlanService(
    ApplicationDbContext context,
    ILogger<SubscriptionPlanService> logger) : ISubscriptionPlanService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<SubscriptionPlanService> _logger = logger;

    public async Task<Result<IReadOnlyList<SubscriptionPlanResponse>>> GetPlansAsync(
        CancellationToken ct = default)
    {
        try
        {
            var plans = await _context.SubscriptionPlans
                .Where(p => p.IsActive)
                .OrderBy(p => p.MonthlyPrice)
                .AsNoTracking()
                .ToListAsync(ct);

            var response = plans.Select(p => new SubscriptionPlanResponse(
                    p.Id, p.Name, p.Description, p.MonthlyPrice, p.MaxCourts, p.IsActive, p.ExpiresAt, GetTimestampFromGuidV7(p.Id)))
                .ToList();

            return Result.Success<IReadOnlyList<SubscriptionPlanResponse>>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving subscription plans.");
            return Result.Failure<IReadOnlyList<SubscriptionPlanResponse>>(SubscriptionErrors.Error);
        }
    }

    public async Task<Result<SubscriptionPlanResponse>> GetPlanAsync(
        string planId, CancellationToken ct = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(planId))
                return Result.Failure<SubscriptionPlanResponse>(SubscriptionErrors.PlanNotFound);

            var plan = await _context.SubscriptionPlans
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == planId, ct);

            if (plan is null)
                return Result.Failure<SubscriptionPlanResponse>(SubscriptionErrors.PlanNotFound);

            var response = new SubscriptionPlanResponse(
                plan.Id, plan.Name, plan.Description, plan.MonthlyPrice, plan.MaxCourts, plan.IsActive, plan.ExpiresAt, GetTimestampFromGuidV7(plan.Id));

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving subscription plan {PlanId}.", planId);
            return Result.Failure<SubscriptionPlanResponse>(SubscriptionErrors.Error);
        }
    }

    public async Task<Result<SubscriptionPlanResponse>> CreatePlanAsync(
        CreateSubscriptionPlanRequest request, CancellationToken ct = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return Result.Failure<SubscriptionPlanResponse>(SubscriptionErrors.InvalidPlan);

            if (request.Price < 0 || request.MaxCourts <= 0)
                return Result.Failure<SubscriptionPlanResponse>(SubscriptionErrors.InvalidPlan);

            var plan = new SubscriptionPlan
            {
                Name = request.Name,
                Description = request.Description,
                MonthlyPrice = request.Price,
                MaxCourts = request.MaxCourts,
                IsActive = true
            };

            await _context.SubscriptionPlans.AddAsync(plan, ct);
            await _context.SaveChangesAsync(ct);

            var response = new SubscriptionPlanResponse(
                plan.Id, plan.Name, plan.Description, plan.MonthlyPrice, plan.MaxCourts, plan.IsActive, plan.ExpiresAt, GetTimestampFromGuidV7(plan.Id));

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating subscription plan.");
            return Result.Failure<SubscriptionPlanResponse>(SubscriptionErrors.Error);
        }
    }

    public async Task<Result<SubscriptionPlanResponse>> UpdatePlanAsync(
        string planId, UpdateSubscriptionPlanRequest request, CancellationToken ct = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(planId))
                return Result.Failure<SubscriptionPlanResponse>(SubscriptionErrors.PlanNotFound);

            if (string.IsNullOrWhiteSpace(request.Name))
                return Result.Failure<SubscriptionPlanResponse>(SubscriptionErrors.InvalidPlan);

            if (request.Price < 0 || request.MaxCourts <= 0)
                return Result.Failure<SubscriptionPlanResponse>(SubscriptionErrors.InvalidPlan);

            var plan = await _context.SubscriptionPlans.FirstOrDefaultAsync(p => p.Id == planId, ct);
            if (plan is null)
                return Result.Failure<SubscriptionPlanResponse>(SubscriptionErrors.PlanNotFound);

            plan.Name = request.Name;
            plan.Description = request.Description;
            plan.MonthlyPrice = request.Price;
            plan.MaxCourts = request.MaxCourts;
            plan.IsActive = request.IsActive;

            await _context.SaveChangesAsync(ct);

            var response = new SubscriptionPlanResponse(
                plan.Id, plan.Name, plan.Description, plan.MonthlyPrice, plan.MaxCourts, plan.IsActive, plan.ExpiresAt, GetTimestampFromGuidV7(plan.Id));

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating subscription plan {PlanId}.", planId);
            return Result.Failure<SubscriptionPlanResponse>(SubscriptionErrors.Error);
        }
    }

    public async Task<Result> ArchivePlanAsync(
        string planId, CancellationToken ct = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(planId))
                return Result.Failure(SubscriptionErrors.PlanNotFound);

            var plan = await _context.SubscriptionPlans.FirstOrDefaultAsync(p => p.Id == planId, ct);
            if (plan is null)
                return Result.Failure(SubscriptionErrors.PlanNotFound);

            plan.IsDeleted = true;
            plan.IsActive = false;

            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while archiving subscription plan {PlanId}.", planId);
            return Result.Failure(SubscriptionErrors.Error);
        }
    }

    private static DateTime GetTimestampFromGuidV7(string guidString)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(guidString))
                return DateTime.UtcNow;

            var cleanGuid = guidString.Replace("-", "");
            if (cleanGuid.Length >= 12)
            {
                var hexTimestamp = cleanGuid.Substring(0, 12);
                var ms = Convert.ToInt64(hexTimestamp, 16);
                return DateTimeOffset.FromUnixTimeMilliseconds(ms).UtcDateTime;
            }
        }
        catch
        {
            // Fallback
        }
        return DateTime.UtcNow;
    }
}