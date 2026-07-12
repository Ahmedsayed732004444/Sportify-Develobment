using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sportiva.Contracts.Subscriptions;
using Sportiva.Entities;
using Sportiva.Persistence;
using Sportiva.Abstractions;

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
                .Select(p => new SubscriptionPlanResponse(
                    p.Id,
                    p.Name,
                    p.Description,
                    p.MonthlyPrice,
                    p.MaxCourts,
                    p.IsActive,
                    p.ExpiresAt,
                    GetTimestampFromGuidV7(p.Id)
                ))
                .AsNoTracking()
                .ToListAsync(ct);

            return Result.Success<IReadOnlyList<SubscriptionPlanResponse>>(plans);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving subscription plans.");
            return Result.Failure<IReadOnlyList<SubscriptionPlanResponse>>(
                new Error("SubscriptionPlans.Error", "An error occurred while processing the subscription plans request", 500));
        }
    }

    public async Task<Result<SubscriptionPlanResponse>> GetPlanAsync(
        string planId, CancellationToken ct = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(planId))
            {
                return Result.Failure<SubscriptionPlanResponse>(
                    new Error("SubscriptionPlans.InvalidId", "Plan ID cannot be empty", 400));
            }

            var plan = await _context.SubscriptionPlans
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == planId, ct);

            if (plan is null)
            {
                return Result.Failure<SubscriptionPlanResponse>(
                    new Error("SubscriptionPlans.NotFound", "The specified subscription plan was not found", 404));
            }

            var response = new SubscriptionPlanResponse(
                plan.Id,
                plan.Name,
                plan.Description,
                plan.MonthlyPrice,
                plan.MaxCourts,
                plan.IsActive,
                plan.ExpiresAt,
                GetTimestampFromGuidV7(plan.Id)
            );

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving subscription plan {PlanId}.", planId);
            return Result.Failure<SubscriptionPlanResponse>(
                new Error("SubscriptionPlans.Error", "An error occurred while processing the subscription plan request", 500));
        }
    }

    public async Task<Result<SubscriptionPlanResponse>> CreatePlanAsync(
        CreateClubSubscriptionRequest request, CancellationToken ct = default)
    {
        try
        {
            // 1. Validation: Null / Empty Fields
            if (string.IsNullOrWhiteSpace(request.ClubId))
            {
                return Result.Failure<SubscriptionPlanResponse>(
                    new Error("SubscriptionPlans.InvalidClubId", "Club ID cannot be empty", 400));
            }

            if (string.IsNullOrWhiteSpace(request.PlanId))
            {
                return Result.Failure<SubscriptionPlanResponse>(
                    new Error("SubscriptionPlans.InvalidPlanId", "Plan ID cannot be empty", 400));
            }

            // 2. Validation: Check Default Dates
            if (request.StartDate == default || request.EndDate == default)
            {
                return Result.Failure<SubscriptionPlanResponse>(
                    new Error("SubscriptionPlans.InvalidDates", "Start date and End date must have valid values", 400));
            }

            // 3. Validation: Start Date in the Past Check (with 5-minute skew tolerance)
            if (request.StartDate < DateTime.UtcNow.AddMinutes(-5))
            {
                return Result.Failure<SubscriptionPlanResponse>(
                    new Error("SubscriptionPlans.StartDateInPast", "Start date cannot be in the past", 400));
            }

            // 4. Validation: End Date after Start Date
            if (request.EndDate <= request.StartDate)
            {
                return Result.Failure<SubscriptionPlanResponse>(
                    new Error("SubscriptionPlans.InvalidDateRange", "End date must be after start date", 400));
            }

            // 5. Validation: Check if the club exists and is active
            var club = await _context.Clubs.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.ClubId, ct);
            if (club is null || club.IsDeleted)
            {
                return Result.Failure<SubscriptionPlanResponse>(
                    new Error("Clubs.NotFound", "The specified club was not found or is deleted", 404));
            }

            if (!club.IsActive)
            {
                return Result.Failure<SubscriptionPlanResponse>(
                    new Error("Clubs.Inactive", "The specified club is currently inactive", 400));
            }

            // 6. Validation: Check if a plan with the same ID already exists
            var exists = await _context.SubscriptionPlans.AnyAsync(p => p.Id == request.PlanId, ct);
            if (exists)
            {
                return Result.Failure<SubscriptionPlanResponse>(
                    new Error("SubscriptionPlans.AlreadyExists", "A subscription plan with the same ID already exists", 400));
            }

            if (request.MonthlyPrice < 0)
            {
                return Result.Failure<SubscriptionPlanResponse>(SubscriptionPlanErrors.InvalidPrice);
            }

            if (request.MaxCourts < 0)
            {
                return Result.Failure<SubscriptionPlanResponse>(SubscriptionPlanErrors.InvalidMaxCourts);
            }

            var planName = string.IsNullOrWhiteSpace(request.Name) ? $"Plan_{request.PlanId}" : request.Name;
            var planDescription = string.IsNullOrWhiteSpace(request.Description) ? $"Created for club {request.ClubId}" : request.Description;

            var plan = new SubscriptionPlan
            {
                Id = request.PlanId,
                Name = planName,
                Description = planDescription,
                MonthlyPrice = request.MonthlyPrice,
                MaxCourts = request.MaxCourts,
                IsActive = true,
                ExpiresAt = request.EndDate
            };

            await _context.SubscriptionPlans.AddAsync(plan, ct);
            await _context.SaveChangesAsync(ct);

            var response = new SubscriptionPlanResponse(
                plan.Id,
                plan.Name,
                plan.Description,
                plan.MonthlyPrice,
                plan.MaxCourts,
                plan.IsActive,
                plan.ExpiresAt,
                GetTimestampFromGuidV7(plan.Id)
            );

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating subscription plan.");
            return Result.Failure<SubscriptionPlanResponse>(SubscriptionPlanErrors.Error);
        }
    }

    public async Task<Result<SubscriptionPlanResponse>> UpdatePlanAsync(
        string planId, CreateClubSubscriptionRequest request, CancellationToken ct = default)
    {
        try
        {
            // 1. Validation: Null / Empty Fields
            if (string.IsNullOrWhiteSpace(planId))
            {
                return Result.Failure<SubscriptionPlanResponse>(SubscriptionPlanErrors.PlanNotFound);
            }

            if (string.IsNullOrWhiteSpace(request.ClubId))
            {
                return Result.Failure<SubscriptionPlanResponse>(
                    new Error("SubscriptionPlans.InvalidClubId", "Club ID cannot be empty", 400));
            }

            // 2. Validation: Check Default Dates
            if (request.StartDate == default || request.EndDate == default)
            {
                return Result.Failure<SubscriptionPlanResponse>(
                    new Error("SubscriptionPlans.InvalidDates", "Start date and End date must have valid values", 400));
            }

            // 3. Validation: End Date after Start Date
            if (request.EndDate <= request.StartDate)
            {
                return Result.Failure<SubscriptionPlanResponse>(
                    new Error("SubscriptionPlans.InvalidDateRange", "End date must be after start date", 400));
            }

            // 4. Validation: Check if plan exists
            var plan = await _context.SubscriptionPlans.FirstOrDefaultAsync(p => p.Id == planId && !p.IsDeleted, ct);
            if (plan is null)
            {
                return Result.Failure<SubscriptionPlanResponse>(SubscriptionPlanErrors.PlanNotFound);
            }

            // 5. Validation: Check if the club exists and is active
            var club = await _context.Clubs.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.ClubId, ct);
            if (club is null || club.IsDeleted)
            {
                return Result.Failure<SubscriptionPlanResponse>(
                    new Error("Clubs.NotFound", "The specified club was not found or is deleted", 404));
            }

            if (!club.IsActive)
            {
                return Result.Failure<SubscriptionPlanResponse>(
                    new Error("Clubs.Inactive", "The specified club is currently inactive", 400));
            }

            if (request.MonthlyPrice < 0)
            {
                return Result.Failure<SubscriptionPlanResponse>(SubscriptionPlanErrors.InvalidPrice);
            }

            if (request.MaxCourts < 0)
            {
                return Result.Failure<SubscriptionPlanResponse>(SubscriptionPlanErrors.InvalidMaxCourts);
            }

            // Update parameters
            plan.ExpiresAt = request.EndDate;
            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                plan.Name = request.Name;
            }
            if (request.Description != null)
            {
                plan.Description = request.Description;
            }
            plan.MonthlyPrice = request.MonthlyPrice;
            plan.MaxCourts = request.MaxCourts;

            await _context.SaveChangesAsync(ct);

            var response = new SubscriptionPlanResponse(
                plan.Id,
                plan.Name,
                plan.Description,
                plan.MonthlyPrice,
                plan.MaxCourts,
                plan.IsActive,
                plan.ExpiresAt,
                GetTimestampFromGuidV7(plan.Id)
            );

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating subscription plan {PlanId}.", planId);
            return Result.Failure<SubscriptionPlanResponse>(SubscriptionPlanErrors.Error);
        }
    }

    public async Task<Result> ArchivePlanAsync(
        string planId, CancellationToken ct = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(planId))
            {
                return Result.Failure(
                    new Error("SubscriptionPlans.InvalidId", "Plan ID cannot be empty", 400));
            }

            var plan = await _context.SubscriptionPlans
                .FirstOrDefaultAsync(p => p.Id == planId, ct);

            if (plan is null)
            {
                return Result.Failure(
                    new Error("SubscriptionPlans.NotFound", "The specified subscription plan was not found", 404));
            }

            plan.IsDeleted = true;
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while archiving subscription plan {PlanId}.", planId);
            return Result.Failure(
                new Error("SubscriptionPlans.Error", "An error occurred while archiving the subscription plan", 500));
        }
    }

    private static DateTime GetTimestampFromGuidV7(string guidString)
    {
        try
        {
            var hex = guidString.Replace("-", "").Substring(0, 12);
            long ms = Convert.ToInt64(hex, 16);
            return DateTimeOffset.FromUnixTimeMilliseconds(ms).UtcDateTime;
        }
        catch
        {
            return DateTime.UtcNow;
        }
    }
}