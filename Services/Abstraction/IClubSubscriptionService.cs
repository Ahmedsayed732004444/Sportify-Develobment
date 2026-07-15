using Sportiva.Contracts.Common;
using Sportiva.Contracts.Subscriptions;

namespace Sportiva.Services;

public interface IClubSubscriptionService
{
    Task<Result<ClubSubscriptionResponse>> GetActiveSubscriptionAsync(
        string userId, string clubId, CancellationToken ct = default);

    Task<Result<PaginatedList<ClubSubscriptionResponse>>> GetSubscriptionHistoryAsync(
        string userId, string clubId, RequestFilters filters, CancellationToken ct = default);

    Task<Result<ClubSubscriptionResponse>> SubscribeAsync(
        string userId, string clubId, CreateClubSubscriptionRequest request, CancellationToken ct = default);

    Task<Result<ClubSubscriptionResponse>> RenewSubscriptionAsync(
        string userId, string clubId, CancellationToken ct = default);

    Task<Result> CancelSubscriptionAsync(
        string userId, string clubId, CancellationToken ct = default);
}
