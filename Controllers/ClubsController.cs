using Sportiva.Contracts.Clubs;
using Sportiva.Contracts.Common;
using Sportiva.Extensions;
using Sportiva.Services;

namespace Sportiva.Controllers;

[Route("clubs")]
[ApiController]
[Authorize]
public class ClubsController(IClubService clubService) : ControllerBase
{
    private readonly IClubService _clubService = clubService;
    //for all users
    // GET /clubs
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetClubs([FromQuery] RequestFilters filters, CancellationToken ct)
    {
        var result = await _clubService.GetClubsAsync(User.GetUserId(), filters, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    //for owners
    // GET /clubs/me
    [HttpGet("me")]
    public async Task<IActionResult> GetMyClubs([FromQuery] RequestFilters filters, CancellationToken ct)
    {
        var result = await _clubService.GetMyClubsAsync(User.GetUserId()!, filters, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    //for all users
    // GET /clubs/{clubId}
    [HttpGet("{clubId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetClub(string clubId, CancellationToken ct)
    {
        var result = await _clubService.GetClubAsync(clubId, User.GetUserId(), ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    //for Admins only
    // POST /clubs
    [HttpPost]
    public async Task<IActionResult> CreateClub([FromForm] CreateClubRequest request, CancellationToken ct)
    {
        var result = await _clubService.CreateClubAsync(User.GetUserId()!, request, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    //for Admins only
    // PUT /clubs/{clubId}
    [HttpPut("{clubId}")]
    public async Task<IActionResult> UpdateClub(string clubId, [FromForm] UpdateClubRequest request, CancellationToken ct)
    {
        var result = await _clubService.UpdateClubAsync(User.GetUserId()!, clubId, request, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    //for Admins only
    // DELETE /clubs/{clubId}
    [HttpDelete("{clubId}")]
    public async Task<IActionResult> DeleteClub(string clubId, CancellationToken ct)
    {
        var result = await _clubService.DeleteClubAsync(User.GetUserId()!, clubId, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
    // for Admins only
    // PATCH /clubs/{clubId}/status
    [HttpPatch("{clubId}/status")]
    public async Task<IActionResult> ToggleClubStatus(string clubId, CancellationToken ct)
    {
        var result = await _clubService.ToggleClubStatusAsync(User.GetUserId()!, clubId, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}