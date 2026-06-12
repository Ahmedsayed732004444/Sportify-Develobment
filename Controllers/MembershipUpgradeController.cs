//using Sportiva.Contracts.Common;
//using Sportiva.Contracts.MembershipUpgrade;
//using Sportiva.Services;

//namespace Sportiva.Controllers
//{
//    [Route("api/membership")]
//    [ApiController]
//    [Authorize]
//    public class MembershipUpgradeController(IMembershipUpgradeService _membershipUpgradeService) : ControllerBase
//    {
//        [HttpPost("upgrade")]
//        public async Task<IActionResult> CreateMembershipUpgradeAsync([FromBody] MembershipUpgradeRequest request, CancellationToken ct)
//        {
//            var result = await _membershipUpgradeService.CreateUpgradeRequestAsync(User.GetUserId()!, request, ct);
//            return result.IsSuccess ? Created() : result.ToProblem();
//        }
//        [HttpPut("upgrade/{id}/approve")]
//        public async Task<IActionResult> ApproveUpgradeRequestAsync(string id, CancellationToken ct)
//        {
//            var result = await _membershipUpgradeService.ApproveUpgradeRequestAsync(id, ct);
//            return result.IsSuccess ? NoContent() : result.ToProblem();
//        }
//        [HttpPut("upgrade/{id}/reject")]
//        public async Task<IActionResult> RejectUpgradeRequestAsync(string id, CancellationToken ct)
//        {
//            var result = await _membershipUpgradeService.RejectUpgradeRequestAsync(id, ct);
//            return result.IsSuccess ? NoContent() : result.ToProblem();
//        }
//        [HttpGet("upgrade")]
//        public async Task<IActionResult> GetUpgradeRequestsAsync([FromQuery] RequestFilters filters, CancellationToken ct)
//        {
//            return Ok(await _membershipUpgradeService.GetAllUpgradeRequestsAsync(filters, ct));
//        }
//        [HttpGet("upgrade/{id}")]
//        public async Task<IActionResult> GetUpgradeRequestDetailsAsync(string id, CancellationToken ct)
//        {
//            var result = await _membershipUpgradeService.GetUpgradeRequestDetailsAsync(id, ct);
//            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
//        }
//    }
//}
