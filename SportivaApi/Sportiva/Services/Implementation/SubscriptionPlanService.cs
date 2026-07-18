using Sportiva.Contracts.Subscriptions;

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
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Price)
                .Select(p => new SubscriptionPlanResponse(
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Price,
                    p.MaxCourts,
                    p.DurationInDays,
                    p.IsActive,
                    p.ExpiresAt,
                    p.CreatedAt))
                .AsNoTracking()
                .ToListAsync(ct);

            return Result.Success<IReadOnlyList<SubscriptionPlanResponse>>(plans);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving subscription plans");
            return Result.Failure<IReadOnlyList<SubscriptionPlanResponse>>(SubscriptionPlanErrors.Error);
        }
    }

    public async Task<Result<SubscriptionPlanResponse>> GetPlanAsync(
        string planId, CancellationToken ct = default)
    {
        try
        {
            var plan = await _context.SubscriptionPlans
                .Where(p => p.Id == planId && !p.IsDeleted)
                .Select(p => new SubscriptionPlanResponse(
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Price,
                    p.MaxCourts,
                    p.DurationInDays,
                    p.IsActive,
                    p.ExpiresAt,
                    p.CreatedAt))
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (plan is null)
                return Result.Failure<SubscriptionPlanResponse>(SubscriptionPlanErrors.PlanNotFound);

            return Result.Success(plan);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving subscription plan {PlanId}", planId);
            return Result.Failure<SubscriptionPlanResponse>(SubscriptionPlanErrors.Error);
        }
    }

    public async Task<Result<SubscriptionPlanResponse>> CreatePlanAsync(
        CreateSubscriptionPlanRequest request, CancellationToken ct = default)
    {
        try
        {
            var plan = new SubscriptionPlan
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                MaxCourts = request.MaxCourts,
                DurationInDays = request.DurationInDays,
                IsActive = true
            };

            _context.SubscriptionPlans.Add(plan);
            await _context.SaveChangesAsync(ct);

            var response = new SubscriptionPlanResponse(
                plan.Id,
                plan.Name,
                plan.Description,
                plan.Price,
                plan.MaxCourts,
                plan.DurationInDays,
                plan.IsActive,
                plan.ExpiresAt,
                plan.CreatedAt);

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating subscription plan {PlanName}", request.Name);
            return Result.Failure<SubscriptionPlanResponse>(SubscriptionPlanErrors.Error);
        }
    }

    public async Task<Result<SubscriptionPlanResponse>> UpdatePlanAsync(
        string planId, UpdateSubscriptionPlanRequest request, CancellationToken ct = default)
    {
        try
        {
            var plan = await _context.SubscriptionPlans
                .FirstOrDefaultAsync(p => p.Id == planId && !p.IsDeleted, ct);

            if (plan is null)
                return Result.Failure<SubscriptionPlanResponse>(SubscriptionPlanErrors.PlanNotFound);

            plan.Name = request.Name;
            plan.Description = request.Description;
            plan.Price = request.Price;
            plan.MaxCourts = request.MaxCourts;
            plan.DurationInDays = request.DurationInDays;
            plan.IsActive = request.IsActive;

            await _context.SaveChangesAsync(ct);

            var response = new SubscriptionPlanResponse(
                plan.Id,
                plan.Name,
                plan.Description,
                plan.Price,
                plan.MaxCourts,
                plan.DurationInDays,
                plan.IsActive,
                plan.ExpiresAt,
                plan.CreatedAt);

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating subscription plan {PlanId}", planId);
            return Result.Failure<SubscriptionPlanResponse>(SubscriptionPlanErrors.Error);
        }
    }

    public async Task<Result> ArchivePlanAsync(
        string planId, CancellationToken ct = default)
    {
        try
        {
            var plan = await _context.SubscriptionPlans
                .FirstOrDefaultAsync(p => p.Id == planId && !p.IsDeleted, ct);

            if (plan is null)
                return Result.Failure(SubscriptionPlanErrors.PlanNotFound);

            plan.IsActive = false;
            plan.IsDeleted = true;

            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while archiving subscription plan {PlanId}", planId);
            return Result.Failure(SubscriptionPlanErrors.Error);
        }
    }
}