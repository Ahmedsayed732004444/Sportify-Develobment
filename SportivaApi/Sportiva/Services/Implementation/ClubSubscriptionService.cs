using Sportiva.Contracts.Common;
using Sportiva.Contracts.Shared.Enums;
using Sportiva.Contracts.Shared.Summaries;
using Sportiva.Contracts.Subscriptions;
using Sportiva.Extensions;

namespace Sportiva.Services;

public class ClubSubscriptionService(
    ApplicationDbContext context,
    ILogger<ClubSubscriptionService> logger) : IClubSubscriptionService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<ClubSubscriptionService> _logger = logger;

    private static readonly string[] AllowedSortColumns = ["StartDate", "EndDate"];

    // ════════════════════════════════════════════════════════════════
    //  Get Active Subscription
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<ClubSubscriptionResponse>> GetActiveSubscriptionAsync(
        string userId, string clubId, CancellationToken ct = default)
    {
        try
        {
            var club = await _context.Clubs
                .FirstOrDefaultAsync(c => c.Id == clubId && !c.IsDeleted, ct);

            if (club is null)
                return Result.Failure<ClubSubscriptionResponse>(ClubErrors.ClubNotFound);

            if (club.OwnerId != userId)
                return Result.Failure<ClubSubscriptionResponse>(ClubErrors.Unauthorized);

            var now = DateTime.UtcNow;

            // ✅ الكويري بترجع الـ Status كـ enum خام (PaymentStatus) من غير أي Cast لـ int،
            // عشان EF يستخدم الـ HasConversion<string> بتاعته صح جوه SQL.
            var subscription = await _context.ClubSubscriptions
                .Where(s => s.ClubId == clubId && !s.IsDeleted &&
                            s.StartDate <= now && s.EndDate >= now)
                .Select(s => new
                {
                    s.Id,
                    Club = new ClubSummary(club.Id, club.Name, club.LogoUrl, club.City, club.Governorate),
                    Plan = new SubscriptionPlanSummary(s.PlanId, s.Plan.Name, s.Plan.Price, s.Plan.MaxCourts),
                    s.StartDate,
                    s.EndDate,
                    PaymentsCount = s.Payments.Count,
                    LastPayment = s.Payments
                        .OrderByDescending(p => p.PaidAt)
                        .Select(p => new { p.Id, p.Amount, p.Status, p.TransactionId, p.PaidAt })
                        .FirstOrDefault()
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (subscription is null)
                return Result.Failure<ClubSubscriptionResponse>(ClubSubscriptionErrors.SubscriptionNotFound);

            // ✅ الـ Cast من enum لـ Dto بيحصل هنا في الميموري C# — آمن تمامًا ومش بيلمس SQL
            var lastPayment = subscription.LastPayment is null
                ? null
                : new SubscriptionPaymentSummary(
                    subscription.LastPayment.Id,
                    subscription.LastPayment.Amount,
                    (PaymentStatusDto)(int)subscription.LastPayment.Status,
                    subscription.LastPayment.TransactionId,
                    subscription.LastPayment.PaidAt);

            var response = new ClubSubscriptionResponse(
                subscription.Id,
                subscription.Club,
                subscription.Plan,
                subscription.StartDate,
                subscription.EndDate,
                IsActive: true,
                subscription.PaymentsCount,
                lastPayment);

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving active subscription for club {ClubId}", clubId);
            return Result.Failure<ClubSubscriptionResponse>(ClubSubscriptionErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Subscription History
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<PaginatedList<ClubSubscriptionResponse>>> GetSubscriptionHistoryAsync(
        string userId, string clubId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var club = await _context.Clubs
                .FirstOrDefaultAsync(c => c.Id == clubId && !c.IsDeleted, ct);

            if (club is null)
                return Result.Failure<PaginatedList<ClubSubscriptionResponse>>(ClubErrors.ClubNotFound);

            if (club.OwnerId != userId)
                return Result.Failure<PaginatedList<ClubSubscriptionResponse>>(ClubErrors.Unauthorized);

            var now = DateTime.UtcNow;

            // ✅ برضو بنجيب الـ Status خام من غير Cast، وبنعمل الـ Pagination على شكل خام
            var query = _context.ClubSubscriptions
                .Where(s => s.ClubId == clubId && !s.IsDeleted)
                .ApplyFilters(filters, allowedSortColumns: AllowedSortColumns)
                .Select(s => new
                {
                    s.Id,
                    s.PlanId,
                    PlanName = s.Plan.Name,
                    PlanPrice = s.Plan.Price,
                    PlanMaxCourts = s.Plan.MaxCourts,
                    s.StartDate,
                    s.EndDate,
                    PaymentsCount = s.Payments.Count,
                    LastPayment = s.Payments
                        .OrderByDescending(p => p.PaidAt)
                        .Select(p => new { p.Id, p.Amount, p.Status, p.TransactionId, p.PaidAt })
                        .FirstOrDefault()
                });

            var paged = await query.AsNoTracking().ToPaginatedListAsync(filters, ct);

            // ✅ الـ mapping النهائي لـ ClubSubscriptionResponse (وفيه الـ Enum Cast) بيحصل
            // بعد ما البيانات كلها اتجابت فعليًا من الداتابيز — مفيش أي SQL بيتترجم هنا
            var result = paged.Select(s => new ClubSubscriptionResponse(
                s.Id,
                new ClubSummary(club.Id, club.Name, club.LogoUrl, club.City, club.Governorate),
                new SubscriptionPlanSummary(s.PlanId, s.PlanName, s.PlanPrice, s.PlanMaxCourts),
                s.StartDate,
                s.EndDate,
                s.StartDate <= now && s.EndDate >= now,
                s.PaymentsCount,
                s.LastPayment is null
                    ? null
                    : new SubscriptionPaymentSummary(
                        s.LastPayment.Id,
                        s.LastPayment.Amount,
                        (PaymentStatusDto)(int)s.LastPayment.Status,
                        s.LastPayment.TransactionId,
                        s.LastPayment.PaidAt)
            ));

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving subscription history for club {ClubId}", clubId);
            return Result.Failure<PaginatedList<ClubSubscriptionResponse>>(ClubSubscriptionErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Subscribe
    //  (مفيش wallet لسه — بنعتبر الدفع تم أوتوماتيك، وهنستبدلها بمنطق
    //   دفع حقيقي لما الـ wallet يتضاف)
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<ClubSubscriptionResponse>> SubscribeAsync(
        string userId, string clubId, CreateClubSubscriptionRequest request, CancellationToken ct = default)
    {
        try
        {
            var club = await _context.Clubs
                .FirstOrDefaultAsync(c => c.Id == clubId && !c.IsDeleted, ct);

            if (club is null)
                return Result.Failure<ClubSubscriptionResponse>(ClubErrors.ClubNotFound);

            if (club.OwnerId != userId)
                return Result.Failure<ClubSubscriptionResponse>(ClubErrors.Unauthorized);

            var plan = await _context.SubscriptionPlans
                .FirstOrDefaultAsync(p => p.Id == request.PlanId && !p.IsDeleted, ct);

            if (plan is null)
                return Result.Failure<ClubSubscriptionResponse>(SubscriptionPlanErrors.PlanNotFound);

            if (!plan.IsActive)
                return Result.Failure<ClubSubscriptionResponse>(ClubSubscriptionErrors.PlanInactive);

            var now = DateTime.UtcNow;

            var hasActiveSubscription = await _context.ClubSubscriptions
                .AnyAsync(s => s.ClubId == clubId && !s.IsDeleted &&
                               s.StartDate <= now && s.EndDate >= now, ct);

            if (hasActiveSubscription)
                return Result.Failure<ClubSubscriptionResponse>(ClubSubscriptionErrors.AlreadySubscribed);

            var subscription = new ClubSubscription
            {
                ClubId = clubId,
                PlanId = plan.Id,
                StartDate = now,
                EndDate = now.AddDays(plan.DurationInDays)
            };

            // TODO: هيتشال لما الـ wallet الحقيقي يتضاف — دلوقتي بنسجل الدفع كـ "تم" مباشرة
            var payment = new SubscriptionPayment
            {
                Amount = plan.Price,
                Status = PaymentStatus.Paid,
                PaidAt = now,
                TransactionId = null
            };

            subscription.Payments.Add(payment);

            _context.ClubSubscriptions.Add(subscription);
            await _context.SaveChangesAsync(ct);

            var response = new ClubSubscriptionResponse(
                subscription.Id,
                new ClubSummary(club.Id, club.Name, club.LogoUrl, club.City, club.Governorate),
                new SubscriptionPlanSummary(plan.Id, plan.Name, plan.Price, plan.MaxCourts),
                subscription.StartDate,
                subscription.EndDate,
                subscription.IsActive,
                PaymentsCount: 1,
                LastPayment: new SubscriptionPaymentSummary(
                    payment.Id, payment.Amount, PaymentStatusDto.Paid, payment.TransactionId, payment.PaidAt));

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while subscribing club {ClubId} to plan {PlanId}", clubId, request.PlanId);
            return Result.Failure<ClubSubscriptionResponse>(ClubSubscriptionErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Renew Subscription
    //  (نفس منطق الدفع الوهمي مؤقتاً)
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<ClubSubscriptionResponse>> RenewSubscriptionAsync(
        string userId, string clubId, CancellationToken ct = default)
    {
        try
        {
            var club = await _context.Clubs
                .FirstOrDefaultAsync(c => c.Id == clubId && !c.IsDeleted, ct);

            if (club is null)
                return Result.Failure<ClubSubscriptionResponse>(ClubErrors.ClubNotFound);

            if (club.OwnerId != userId)
                return Result.Failure<ClubSubscriptionResponse>(ClubErrors.Unauthorized);

            var latest = await _context.ClubSubscriptions
                .Where(s => s.ClubId == clubId && !s.IsDeleted)
                .OrderByDescending(s => s.EndDate)
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(ct);

            if (latest is null)
                return Result.Failure<ClubSubscriptionResponse>(ClubSubscriptionErrors.NoSubscriptionToRenew);

            if (!latest.Plan.IsActive)
                return Result.Failure<ClubSubscriptionResponse>(ClubSubscriptionErrors.PlanInactive);

            var now = DateTime.UtcNow;
            var newStart = latest.IsActive ? latest.EndDate : now;
            var newEnd = newStart.AddDays(latest.Plan.DurationInDays);

            var subscription = new ClubSubscription
            {
                ClubId = clubId,
                PlanId = latest.PlanId,
                StartDate = newStart,
                EndDate = newEnd
            };

            // TODO: هيتشال لما الـ wallet الحقيقي يتضاف
            var payment = new SubscriptionPayment
            {
                Amount = latest.Plan.Price,
                Status = PaymentStatus.Paid,
                PaidAt = now,
                TransactionId = null
            };

            subscription.Payments.Add(payment);

            _context.ClubSubscriptions.Add(subscription);
            await _context.SaveChangesAsync(ct);

            var response = new ClubSubscriptionResponse(
                subscription.Id,
                new ClubSummary(club.Id, club.Name, club.LogoUrl, club.City, club.Governorate),
                new SubscriptionPlanSummary(latest.Plan.Id, latest.Plan.Name, latest.Plan.Price, latest.Plan.MaxCourts),
                subscription.StartDate,
                subscription.EndDate,
                subscription.IsActive,
                PaymentsCount: 1,
                LastPayment: new SubscriptionPaymentSummary(
                    payment.Id, payment.Amount, PaymentStatusDto.Paid, payment.TransactionId, payment.PaidAt));

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while renewing subscription for club {ClubId}", clubId);
            return Result.Failure<ClubSubscriptionResponse>(ClubSubscriptionErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Cancel Subscription
    // ════════════════════════════════════════════════════════════════

    public async Task<Result> CancelSubscriptionAsync(
        string userId, string clubId, CancellationToken ct = default)
    {
        try
        {
            var club = await _context.Clubs
                .FirstOrDefaultAsync(c => c.Id == clubId && !c.IsDeleted, ct);

            if (club is null)
                return Result.Failure(ClubErrors.ClubNotFound);

            if (club.OwnerId != userId)
                return Result.Failure(ClubErrors.Unauthorized);

            var now = DateTime.UtcNow;

            var subscription = await _context.ClubSubscriptions
                .FirstOrDefaultAsync(s => s.ClubId == clubId && !s.IsDeleted &&
                                           s.StartDate <= now && s.EndDate >= now, ct);

            if (subscription is null)
                return Result.Failure(ClubSubscriptionErrors.SubscriptionNotFound);

            subscription.IsDeleted = true;
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while cancelling subscription for club {ClubId}", clubId);
            return Result.Failure(ClubSubscriptionErrors.Error);
        }
    }
}