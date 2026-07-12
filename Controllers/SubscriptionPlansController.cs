using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    private const int ClientClosedRequestStatusCode = 499;

    // GET /subscription-plans
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetPlans(CancellationToken ct)
    {
        try
        {
            var result = await _subscriptionPlanService.GetPlansAsync(ct);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(ClientClosedRequestStatusCode, new { message = "Request was cancelled by the client." });
        }
    }

    // GET /subscription-plans/{planId}
    [HttpGet("{planId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPlan(string planId, CancellationToken ct)
    {
        try
        {
            var result = await _subscriptionPlanService.GetPlanAsync(planId, ct);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(ClientClosedRequestStatusCode, new { message = "Request was cancelled by the client." });
        }
    }

    // POST /subscription-plans
    [HttpPost]
    public async Task<IActionResult> CreatePlan([FromBody] CreateSubscriptionPlanRequest request, CancellationToken ct)
    {
        try
        {
            var result = await _subscriptionPlanService.CreatePlanAsync(request, ct);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(ClientClosedRequestStatusCode, new { message = "Request was cancelled by the client." });
        }
    }

    // PUT /subscription-plans/{planId}
    [HttpPut("{planId}")]
    public async Task<IActionResult> UpdatePlan(string planId, [FromBody] UpdateSubscriptionPlanRequest request, CancellationToken ct)
    {
        try
        {
            var result = await _subscriptionPlanService.UpdatePlanAsync(planId, request, ct);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(ClientClosedRequestStatusCode, new { message = "Request was cancelled by the client." });
        }
    }

    // DELETE /subscription-plans/{planId}
    [HttpDelete("{planId}")]
    public async Task<IActionResult> ArchivePlan(string planId, CancellationToken ct)
    {
        try
        {
            var result = await _subscriptionPlanService.ArchivePlanAsync(planId, ct);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(ClientClosedRequestStatusCode, new { message = "Request was cancelled by the client." });
        }
    }
}
