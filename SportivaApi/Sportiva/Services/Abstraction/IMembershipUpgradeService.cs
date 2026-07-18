using Sportiva.Contracts.Common;
using Sportiva.Contracts.Memberships;
using Sportiva.Enums;

namespace Sportiva.Services;

public interface IMembershipUpgradeService
{
    Task<Result<MembershipUpgradeResponse>> GetUpgradeRequestAsync(
        string requestId, CancellationToken ct = default);

    Task<Result<PaginatedList<MembershipUpgradeResponse>>> GetUpgradeRequestsAsync(
        RequestFilters filters, RequestStatus? status = null, CancellationToken ct = default);

    Task<Result<MembershipUpgradeResponse>> GetMyUpgradeRequestAsync(
        string userId, CancellationToken ct = default);

    Task<Result<MembershipUpgradeResponse>> SubmitUpgradeRequestAsync(
        string userId, CreateMembershipUpgradeRequest request, CancellationToken ct = default);

    Task<Result> ReviewUpgradeRequestAsync(
        string adminId, string requestId, ReviewMembershipUpgradeRequest request, CancellationToken ct = default);
}
