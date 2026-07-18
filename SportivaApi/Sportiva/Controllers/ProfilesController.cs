using Sportiva.Contracts.Common;
using Sportiva.Contracts.Users;
using Sportiva.Extensions;
using Sportiva.Services;

namespace Sportiva.Controllers;

[Route("profiles")]
[ApiController]
[Authorize]
public class ProfilesController(IProfileService profileService) : ControllerBase
{
    private readonly IProfileService _profileService = profileService;

    // GET /profiles/{userId}
    [HttpGet("{userId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProfile(string userId, CancellationToken ct)
    {
        var result = await _profileService.GetProfileAsync(userId, User.GetUserId(), ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    // PUT /profiles/me/info
    [HttpPut("me/info")]
    public async Task<IActionResult> UpdateProfileInfo([FromBody] UpdateProfileInfoRequest request, CancellationToken ct)
    {
        var result = await _profileService.UpdateProfileInfoAsync(User.GetUserId()!, request, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    // PUT /profiles/me/photo
    [HttpPut("me/photo")]
    public async Task<IActionResult> UpdateProfilePhoto([FromForm] UpdateProfilePhotoRequest request, CancellationToken ct)
    {
        var result = await _profileService.UpdateProfilePhotoAsync(User.GetUserId()!, request, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    // PUT /profiles/me/cover
    [HttpPut("me/cover")]
    public async Task<IActionResult> UpdateProfileCover([FromForm] UpdateProfileCoverRequest request, CancellationToken ct)
    {
        var result = await _profileService.UpdateProfileCoverAsync(User.GetUserId()!, request, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    // POST /profiles/{userId}/follow
    [HttpPost("{userId}/follow")]
    public async Task<IActionResult> ToggleFollow(string userId, CancellationToken ct)
    {
        var result = await _profileService.ToggleFollowAsync(User.GetUserId()!, userId, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    // GET /profiles/{userId}/followers
    [HttpGet("{userId}/followers")]
    [AllowAnonymous]
    public async Task<IActionResult> GetFollowers(string userId, [FromQuery] RequestFilters filters, CancellationToken ct)
    {
        var result = await _profileService.GetFollowersAsync(userId, User.GetUserId(), filters, ct);
        return Ok(result);
    }

    // GET /profiles/{userId}/following
    [HttpGet("{userId}/following")]
    [AllowAnonymous]
    public async Task<IActionResult> GetFollowing(string userId, [FromQuery] RequestFilters filters, CancellationToken ct)
    {
        var result = await _profileService.GetFollowingAsync(userId, User.GetUserId(), filters, ct);
        return Ok(result);
    }

    // GET /profiles
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> SearchUsers([FromQuery] RequestFilters filters, CancellationToken ct)
    {
        var result = await _profileService.SearchUsersAsync(User.GetUserId(), filters, ct);
        return Ok(result);
    }

    // POST /profiles/{userId}/suspend
    [HttpPost("{userId}/suspend")]
    [Authorize(Roles = DefaultRoles.Admin.Name)]
    public async Task<IActionResult> SuspendUser(string userId, CancellationToken ct)
    {
        var result = await _profileService.ToggleUserSuspensionAsync(User.GetUserId()!, userId, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}