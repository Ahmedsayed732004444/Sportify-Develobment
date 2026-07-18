using Sportiva.Contracts.Subscriptions;
using Sportiva.Extensions;
using Sportiva.Services;

namespace Sportiva.Controllers;

[Route("subscription-plans")]
[ApiController]
[Authorize]
public class SubscriptionPlansController(ISubscriptionPlanService subscriptionPlanService) : ControllerBase
{
    private readonly ISubscriptionPlanService _subscriptionPlanService = subscriptionPlanService;

    //for all users (صاحب النادي محتاج يشوف الخطط عشان يختار ويشترك)
    // GET /subscription-plans
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetPlans(CancellationToken ct)
    {
        var result = await _subscriptionPlanService.GetPlansAsync(ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    //for all users
    // GET /subscription-plans/{planId}
    [HttpGet("{planId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPlan(string planId, CancellationToken ct)
    {
        var result = await _subscriptionPlanService.GetPlanAsync(planId, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    //for Admins only
    // POST /subscription-plans
    [HttpPost]
    public async Task<IActionResult> CreatePlan([FromBody] CreateSubscriptionPlanRequest request, CancellationToken ct)
    {
        var result = await _subscriptionPlanService.CreatePlanAsync(request, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    //for Admins only
    // PUT /subscription-plans/{planId}
    [HttpPut("{planId}")]
    public async Task<IActionResult> UpdatePlan(string planId, [FromBody] UpdateSubscriptionPlanRequest request, CancellationToken ct)
    {
        var result = await _subscriptionPlanService.UpdatePlanAsync(planId, request, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    //for Admins only
    // DELETE /subscription-plans/{planId}   (soft delete/archive)
    [HttpDelete("{planId}")]
    public async Task<IActionResult> ArchivePlan(string planId, CancellationToken ct)
    {
        var result = await _subscriptionPlanService.ArchivePlanAsync(planId, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
