using Sportiva.Contracts.Common;
using Sportiva.Contracts.Posts;
using Sportiva.Extensions;
using Sportiva.Services;

namespace Sportiva.Controllers;

[Route("posts")]
[ApiController]
[Authorize]
public class PostsController(IPostService postService) : ControllerBase
{
    private readonly IPostService _postService = postService;
    private const int ClientClosedRequestStatusCode = 499;

    // POST /posts
    [HttpPost]
    public async Task<IActionResult> CreatePost([FromForm] CreatePostRequest request, CancellationToken ct)
    {
        try
        {
            var result = await _postService.CreatePostAsync(User.GetUserId()!, request, ct);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(ClientClosedRequestStatusCode, new { message = "Request was cancelled by the client." });
        }
    }

    // DELETE /posts/{postId}
    [HttpDelete("{postId}")]
    public async Task<IActionResult> DeletePost(string postId, CancellationToken ct)
    {
        try
        {
            var result = await _postService.SoftDeletePostAsync(User.GetUserId()!, postId, ct);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(ClientClosedRequestStatusCode, new { message = "Request was cancelled by the client." });
        }
    }

    // PUT /posts/{postId}
    [HttpPut("{postId}")]
    public async Task<IActionResult> UpdatePost(string postId, [FromBody] UpdatePostRequest request, CancellationToken ct)
    {
        try
        {
            var result = await _postService.UpdatePostAsync(User.GetUserId()!, postId, request, ct);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(ClientClosedRequestStatusCode, new { message = "Request was cancelled by the client." });
        }
    }

    // GET /posts/{postId}
    [HttpGet("{postId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPost(string postId, CancellationToken ct)
    {
        try
        {
            var result = await _postService.GetPostAsync(postId, User.GetUserId(), ct);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(ClientClosedRequestStatusCode, new { message = "Request was cancelled by the client." });
        }
    }

    // GET /posts/user/{userId}
    [HttpGet("user/{userId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPostsByUser(string userId, [FromQuery] RequestFilters filters, CancellationToken ct)
    {
        try
        {
            var result = await _postService.GetPostsByUserAsync(userId, User.GetUserId(), filters, ct);
            return Ok(result);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(ClientClosedRequestStatusCode, new { message = "Request was cancelled by the client." });
        }
    }

    // GET /posts
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetPosts([FromQuery] RequestFilters filters, CancellationToken ct)
    {
        try
        {
            var result = await _postService.GetPostsAsync(User.GetUserId(), filters, ct);
            return Ok(result);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(ClientClosedRequestStatusCode, new { message = "Request was cancelled by the client." });
        }
    }

    // POST /posts/{postId}/like
    [HttpPost("{postId}/like")]
    public async Task<IActionResult> ToggleLike(string postId, CancellationToken ct)
    {
        try
        {
            var result = await _postService.ToggleLikeAsync(User.GetUserId()!, postId, ct);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(ClientClosedRequestStatusCode, new { message = "Request was cancelled by the client." });
        }
    }

    // GET /posts/{postId}/likers
    [HttpGet("{postId}/likers")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPostLikers(string postId, [FromQuery] RequestFilters filters, CancellationToken ct)
    {
        try
        {
            var result = await _postService.GetPostLikersAsync(postId, filters, ct);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(ClientClosedRequestStatusCode, new { message = "Request was cancelled by the client." });
        }
    }
}