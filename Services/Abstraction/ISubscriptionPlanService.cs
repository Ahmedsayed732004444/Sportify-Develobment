using Sportiva.Contracts.Subscriptions;

namespace Sportiva.Services;

public interface ISubscriptionPlanService
{
    Task<Result<IReadOnlyList<SubscriptionPlanResponse>>> GetPlansAsync(
        CancellationToken ct = default);

    Task<Result<SubscriptionPlanResponse>> GetPlanAsync(
        string planId, CancellationToken ct = default);

    Task<Result<SubscriptionPlanResponse>> CreatePlanAsync(
        CreateSubscriptionPlanRequest request, CancellationToken ct = default);

    Task<Result<SubscriptionPlanResponse>> UpdatePlanAsync(
        string planId, UpdateSubscriptionPlanRequest request, CancellationToken ct = default);

    Task<Result> ArchivePlanAsync(
        string planId, CancellationToken ct = default);
}
