using Sportiva.Contracts.Common;
using Sportiva.Contracts.Matches;
using Sportiva.Extensions;
using Sportiva.Services;

namespace Sportiva.Controllers;

[Route("friendly-matches/{matchId}/join-requests")]
[ApiController]
[Authorize]
public class MatchJoinRequestsController(IMatchJoinRequestService requestService) : ControllerBase
{
    private readonly IMatchJoinRequestService _requestService = requestService;

    [HttpGet]
    public async Task<IActionResult> GetMatchJoinRequests(string matchId, [FromQuery] RequestFilters filters, CancellationToken ct)
    {
        var result = await _requestService.GetMatchJoinRequestsAsync(User.GetUserId()!, matchId, filters, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("~/friendly-matches/my-join-requests")]
    public async Task<IActionResult> GetMyJoinRequests([FromQuery] RequestFilters filters, [FromQuery] JoinRequestStatus? status, CancellationToken ct)
    {
        var result = await _requestService.GetMyJoinRequestsAsync(User.GetUserId()!, filters, status, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("{requestId}")]
    public async Task<IActionResult> GetJoinRequest(string matchId, string requestId, CancellationToken ct)
    {
        var result = await _requestService.GetJoinRequestAsync(User.GetUserId()!, matchId, requestId, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost]
    public async Task<IActionResult> RequestToJoin(string matchId, [FromBody] JoinMatchRequest request, CancellationToken ct)
    {
        var result = await _requestService.RequestToJoinAsync(User.GetUserId()!, matchId, request, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{requestId}/accept")]
    public async Task<IActionResult> AcceptJoinRequest(string matchId, string requestId, CancellationToken ct)
    {
        var result = await _requestService.AcceptJoinRequestAsync(User.GetUserId()!, matchId, requestId, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPut("{requestId}/reject")]
    public async Task<IActionResult> RejectJoinRequest(string matchId, string requestId, CancellationToken ct)
    {
        var result = await _requestService.RejectJoinRequestAsync(User.GetUserId()!, matchId, requestId, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPost("{requestId}/withdraw")]
    public async Task<IActionResult> WithdrawJoinRequest(string matchId, string requestId, CancellationToken ct)
    {
        var result = await _requestService.WithdrawJoinRequestAsync(User.GetUserId()!, matchId, requestId, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
