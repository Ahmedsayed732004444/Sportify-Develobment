//using Sportiva.Services;

//namespace Sportiva.Controllers;

//[Route("api/subscrip")]
//[ApiController]
//[Authorize]
//public class SubscriptionController(ISubscriptionPlanService _subService) : ControllerBase
//{
//    [HttpPost("plan")]
//    public async Task<IActionResult> CreateSubscriptionPlan([FromBody] CreateSubscriptionPlanRequest request, CancellationToken ct)
//    {
//        var response = await _subService.CreateSubscriptionPlanAsync(request, ct);
//        return response.IsSuccess ? Ok(response.Value) : response.ToProblem();
//    }
//    [HttpPut("plan/{planId}")]
//    public async Task<IActionResult> UpdateSubscriptionPlan(string planId, [FromBody] UpdateSubscriptionPlanRequest request, CancellationToken ct)
//    {
//        var response = await _subService.UpdateSubscriptionPlanAsync(planId, request, ct);
//        return response.IsSuccess ? Created() : response.ToProblem();
//    }
//    [HttpDelete("plan/{planId}")]
//    public async Task<IActionResult> SoftDeleteSubscriptionPlan(string planId, CancellationToken ct)
//    {
//        var response = await _subService.SoftDeleteSubscriptionPlanAsync(planId, ct);
//        return response.IsSuccess ? NoContent() : response.ToProblem();
//    }
//    [HttpPut("plan/{planId}/toggle")]
//    public async Task<IActionResult> ToggleStatus(string planId, CancellationToken ct)
//    {
//        var response = await _subService.ToggleStatus(planId, ct);
//        return response.IsSuccess ? NoContent() : response.ToProblem();
//    }
//    [HttpGet("plan/{planId}")]
//    [AllowAnonymous]
//    public async Task<IActionResult> GetSubscriptionPlan(string planId, CancellationToken ct)
//    {
//        var response = await _subService.GetSubscriptionPlanAsync(planId, ct);
//        return response.IsSuccess ? Ok(response.Value) : response.ToProblem();
//    }
//    [HttpGet("plans")]
//    [AllowAnonymous]
//    public async Task<IActionResult> GetAllSubscriptionPlan(CancellationToken ct)
//    {
//        var response = await _subService.GetAllSubscriptionPlanAsync(ct);
//        return Ok(response);
//    }

//}
