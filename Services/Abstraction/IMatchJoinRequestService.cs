using Sportiva.Contracts.Common;
using Sportiva.Contracts.Matches;
using Sportiva.Enums;

namespace Sportiva.Services;

public interface IMatchJoinRequestService
{
    Task<Result<MatchJoinRequestResponse>> RequestToJoinAsync(
        string userId, string matchId, JoinMatchRequest request, CancellationToken ct = default);

    Task<Result<MatchJoinRequestResponse>> GetJoinRequestAsync(
        string userId, string matchId, string requestId, CancellationToken ct = default);

    Task<Result<PaginatedList<MatchJoinRequestResponse>>> GetMatchJoinRequestsAsync(
        string userId, string matchId, RequestFilters filters, CancellationToken ct = default);

    Task<Result<PaginatedList<MatchJoinRequestResponse>>> GetMyJoinRequestsAsync(
        string userId, RequestFilters filters, JoinRequestStatus? status = null, CancellationToken ct = default);

    Task<Result> AcceptJoinRequestAsync(
        string userId, string matchId, string requestId, CancellationToken ct = default);

    Task<Result> RejectJoinRequestAsync(
        string userId, string matchId, string requestId, CancellationToken ct = default);

    Task<Result> WithdrawJoinRequestAsync(
        string userId, string matchId, string requestId, CancellationToken ct = default);
}
