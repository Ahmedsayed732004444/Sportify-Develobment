using Sportiva.Contracts.Common;
using Sportiva.Contracts.Posts;
using Sportiva.Extensions;
using Sportiva.Services;

namespace Sportiva.Controllers;

[Route("posts/{postId}/comments")]
[ApiController]
[Authorize]
public class CommentsController(ICommentService commentService) : ControllerBase
{
    private readonly ICommentService _commentService = commentService;

    // POST /posts/{postId}/comments
    [HttpPost]
    public async Task<IActionResult> CreateComment(string postId, [FromBody] CreateCommentRequest request, CancellationToken ct)
    {
        request = request with { PostId = postId };
        var result = await _commentService.CreateCommentAsync(User.GetUserId()!, request, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    // PUT /posts/{postId}/comments/{commentId}
    [HttpPut("{commentId}")]
    public async Task<IActionResult> UpdateComment(string commentId, [FromBody] UpdateCommentRequest request, CancellationToken ct)
    {
        var result = await _commentService.UpdateCommentAsync(User.GetUserId()!, commentId, request.Content, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    // DELETE /posts/{postId}/comments/{commentId}
    [HttpDelete("{commentId}")]
    public async Task<IActionResult> DeleteComment(string commentId, CancellationToken ct)
    {
        var result = await _commentService.DeleteCommentAsync(User.GetUserId()!, commentId, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    // GET /posts/{postId}/comments
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetPostComments(string postId, [FromQuery] RequestFilters filters, CancellationToken ct)
    {
        var result = await _commentService.GetPostCommentsAsync(postId, User.GetUserId(), filters, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    // POST /posts/{postId}/comments/{commentId}/like
    [HttpPost("{commentId}/like")]
    public async Task<IActionResult> ToggleCommentLike(string commentId, CancellationToken ct)
    {
        var result = await _commentService.ToggleCommentLikeAsync(User.GetUserId()!, commentId, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    // ════════════════════════════════════════════════════════════════
    //  Replies
    // ════════════════════════════════════════════════════════════════

    // POST /posts/{postId}/comments/{commentId}/replies
    [HttpPost("{commentId}/replies")]
    public async Task<IActionResult> CreateReply(string commentId, [FromBody] CreateReplyRequest request, CancellationToken ct)
    {
        request = request with { CommentId = commentId };
        var result = await _commentService.CreateReplyAsync(User.GetUserId()!, request, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    // PUT /posts/{postId}/comments/{commentId}/replies/{replyId}
    [HttpPut("{commentId}/replies/{replyId}")]
    public async Task<IActionResult> UpdateReply(string replyId, [FromBody] UpdateReplyRequest request, CancellationToken ct)
    {
        var result = await _commentService.UpdateReplyAsync(User.GetUserId()!, replyId, request.Content, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    // DELETE /posts/{postId}/comments/{commentId}/replies/{replyId}
    [HttpDelete("{commentId}/replies/{replyId}")]
    public async Task<IActionResult> DeleteReply(string replyId, CancellationToken ct)
    {
        var result = await _commentService.DeleteReplyAsync(User.GetUserId()!, replyId, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    // GET /posts/{postId}/comments/{commentId}/replies
    [HttpGet("{commentId}/replies")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCommentReplies(string commentId, [FromQuery] RequestFilters filters, CancellationToken ct)
    {
        var result = await _commentService.GetCommentRepliesAsync(commentId, User.GetUserId(), filters, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    // POST /posts/{postId}/comments/{commentId}/replies/{replyId}/like
    [HttpPost("{commentId}/replies/{replyId}/like")]
    public async Task<IActionResult> ToggleReplyLike(string replyId, CancellationToken ct)
    {
        var result = await _commentService.ToggleReplyLikeAsync(User.GetUserId()!, replyId, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}