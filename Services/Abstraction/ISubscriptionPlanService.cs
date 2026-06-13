using Sportiva.Contracts.Common;
using Sportiva.Contracts.Subscriptions;

namespace Sportiva.Services;

public interface ISubscriptionPlanService
{
    // ── Queries ────────────────────────────────────────────────────
    Task<Result<SubscriptionPlanResponse>> GetPlanAsync(
        string planId, CancellationToken ct = default);

    Task<PaginatedList<SubscriptionPlanResponse>> GetPlansAsync(
        RequestFilters filters, CancellationToken ct = default);

    // ── Admin CRUD ─────────────────────────────────────────────────
    //Task<Result<SubscriptionPlanResponse>> CreatePlanAsync(
    //    CreateSubscriptionPlanRequest request,
    //    CancellationToken ct = default);

    //Task<Result<SubscriptionPlanResponse>> UpdatePlanAsync(
    //    string planId, UpdateSubscriptionPlanRequest request,
    //CancellationToken ct = default);

    Task<Result> DeletePlanAsync(
        string planId, CancellationToken ct = default);
}
