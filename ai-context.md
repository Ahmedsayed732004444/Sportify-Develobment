This file is a merged representation of a subset of the codebase, containing specifically included files and files not matching ignore patterns, combined into a single document by Repomix.

# File Summary

## Purpose
This file contains a packed representation of a subset of the repository's contents that is considered the most important context.
It is designed to be easily consumable by AI systems for analysis, code review,
or other automated processes.

## File Format
The content is organized as follows:
1. This summary section
2. Repository information
3. Directory structure
4. Repository files (if enabled)
5. Multiple file entries, each consisting of:
  a. A header with the file path (## File: path/to/file)
  b. The full contents of the file in a code block

## Usage Guidelines
- This file should be treated as read-only. Any changes should be made to the
  original repository files, not this packed version.
- When processing this file, use the file path to distinguish
  between different files in the repository.
- Be aware that this file may contain sensitive information. Handle it with
  the same level of security as you would the original repository.

## Notes
- Some files may have been excluded based on .gitignore rules and Repomix's configuration
- Binary files are not included in this packed representation. Please refer to the Repository Structure section for a complete list of file paths, including binary files
- Only files matching these patterns are included: Contracts/Posts/**, Controllers/PostsController.cs, Controllers/CommentsController.cs, Services/IPostService.cs, Services/ICommentService.cs, Errors/PostErrors.cs, Errors/CommentErrors.cs
- Files matching these patterns are excluded: ai-context.md, repomix-output.xml, keys/**, wwwroot/**, **/*.xml, **/*.csproj, **/*.sln, **/*.user, **/*.designer.cs, **/*.g.cs, **/bin/**, **/obj/**, **/.vs/**, **/Migrations/**
- Files matching patterns in .gitignore are excluded
- Files matching default ignore patterns are excluded
- Files are sorted by Git change count (files with more changes are at the bottom)

# Directory Structure
```
Contracts/Posts/CommentReplyResponse.cs
Contracts/Posts/CreateCommentRequest.cs
Contracts/Posts/CreatePostRequest.cs
Contracts/Posts/CreateReplyRequest.cs
Contracts/Posts/PostCommentResponse.cs
Contracts/Posts/PostLikerResponse.cs
Contracts/Posts/PostResponse.cs
Contracts/Posts/ToggleCommentLikeResponse.cs
Contracts/Posts/ToggleLikeResponse.cs
Contracts/Posts/ToggleReplyLikeResponse.cs
Contracts/Posts/UpdateCommentRequest.cs
Contracts/Posts/UpdatePostRequest.cs
Contracts/Posts/UpdateReplyRequest.cs
Controllers/CommentsController.cs
Controllers/PostsController.cs
Errors/CommentErrors.cs
Errors/PostErrors.cs
Services/ICommentService.cs
Services/IPostService.cs
```

# Files

## File: Contracts/Posts/CommentReplyResponse.cs
```csharp
using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Posts;

public record CommentReplyResponse(
    string      ReplyId,
    string      CommentId,
    string      Content,
    UserSummary Author,
    bool        IsOwner,
    bool        ILiked,
    int         LikesCount,
    DateTime    CreatedAt
);
```

## File: Contracts/Posts/CreateCommentRequest.cs
```csharp
namespace Sportiva.Contracts.Posts;

public record CreateCommentRequest(
    string PostId,
    string Content
);
```

## File: Contracts/Posts/CreatePostRequest.cs
```csharp
namespace Sportiva.Contracts.Posts;

public record CreatePostRequest(
    string Content,
    IFormFile? File
);
```

## File: Contracts/Posts/CreateReplyRequest.cs
```csharp
namespace Sportiva.Contracts.Posts;

public record CreateReplyRequest(
    string CommentId,
    string Content
);
```

## File: Contracts/Posts/PostCommentResponse.cs
```csharp
using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Posts;

public record PostCommentResponse(
    string      CommentId,
    string      PostId,
    string      Content,
    UserSummary Author,
    bool        IsOwner,
    bool        ILiked,
    int         LikesCount,
    int         RepliesCount,
    DateTime    CreatedAt
);
```

## File: Contracts/Posts/PostLikerResponse.cs
```csharp
namespace Sportiva.Contracts.Posts;

public record PostLikerResponse(
    string   UserId,
    string   FullName,
    string?  ProfilePictureUrl,
    DateTime LikedAt
);
```

## File: Contracts/Posts/PostResponse.cs
```csharp
using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Posts;

public record PostResponse(
    string      PostId,
    string      Content,
    string?     FileUrl,
    UserSummary Author,
    bool        IsOwner,
    bool        ILiked,
    int         LikesCount,
    int         CommentsCount,
    DateTime    CreatedAt
);
```

## File: Contracts/Posts/ToggleCommentLikeResponse.cs
```csharp
namespace Sportiva.Contracts.Posts;

public record ToggleCommentLikeResponse(
 string CommentId,
 bool IsLiked,
 int LikesCount
);
```

## File: Contracts/Posts/ToggleLikeResponse.cs
```csharp
namespace Sportiva.Contracts.Posts;

public record ToggleLikeResponse(
    string PostId,
    bool   ILiked,      // true = now liked, false = now unliked
    int    LikesCount
);
```

## File: Contracts/Posts/ToggleReplyLikeResponse.cs
```csharp
namespace Sportiva.Contracts.Posts;

public record ToggleReplyLikeResponse(
 string ReplyId,
 bool IsLiked,
 int LikesCount
);
```

## File: Contracts/Posts/UpdateCommentRequest.cs
```csharp
namespace Sportiva.Contracts.Posts;

public record UpdateCommentRequest(string Content);
```

## File: Contracts/Posts/UpdatePostRequest.cs
```csharp
namespace Sportiva.Contracts.Posts;

public record UpdatePostRequest(
    string Content
);
```

## File: Contracts/Posts/UpdateReplyRequest.cs
```csharp
namespace Sportiva.Contracts.Posts;

public record UpdateReplyRequest(string Content);
```

## File: Controllers/CommentsController.cs
```csharp
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
```

## File: Controllers/PostsController.cs
```csharp
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
```

## File: Errors/CommentErrors.cs
```csharp
namespace Sportiva.Errors;

public record CommentErrors
{
    public static readonly Error Error =
        new("Comments.Error", "An error occurred while processing the comment", StatusCodes.Status500InternalServerError);

    public static readonly Error CommentNotFound =
        new("Comments.NotFound", "The specified comment was not found", StatusCodes.Status404NotFound);

    public static readonly Error Unauthorized =
        new("Comments.Unauthorized", "You are not authorized to perform this action", StatusCodes.Status403Forbidden);

    public static readonly Error AlreadyLiked =
        new("Comments.AlreadyLiked", "You have already liked this comment", StatusCodes.Status400BadRequest);

    public static readonly Error LikeNotFound =
        new("Comments.LikeNotFound", "The specified like was not found", StatusCodes.Status404NotFound);
}

public record ReplyErrors
{
    public static readonly Error Error =
        new("Replies.Error", "An error occurred while processing the reply", StatusCodes.Status500InternalServerError);

    public static readonly Error ReplyNotFound =
        new("Replies.NotFound", "The specified reply was not found", StatusCodes.Status404NotFound);

    public static readonly Error Unauthorized =
        new("Replies.Unauthorized", "You are not authorized to perform this action", StatusCodes.Status403Forbidden);

    public static readonly Error AlreadyLiked =
        new("Replies.AlreadyLiked", "You have already liked this reply", StatusCodes.Status400BadRequest);

    public static readonly Error LikeNotFound =
        new("Replies.LikeNotFound", "The specified like was not found", StatusCodes.Status404NotFound);
}
```

## File: Errors/PostErrors.cs
```csharp
namespace Sportiva.Errors;
public class PostErrors
{
    public static readonly Error Error =
        new("Posts.Error", "An error occurred while processing the post", StatusCodes.Status500InternalServerError);
    public static readonly Error PostNotFound =
        new("Posts.NotFound", "The specified post was not found", StatusCodes.Status404NotFound);
    public static readonly Error Unauthorized =
        new("Posts.Unauthorized", "You are not authorized to perform this action", StatusCodes.Status403Forbidden);
    public static readonly Error LikeNotFound =
        new("Posts.LikeNotFound", "The specified like was not found", StatusCodes.Status404NotFound);
    public static readonly Error AlreadyLiked =
        new("Posts.AlreadyLiked", "You have already liked this post", StatusCodes.Status400BadRequest);
}
```

## File: Services/ICommentService.cs
```csharp
using Sportiva.Contracts.Common;
using Sportiva.Contracts.Posts;

namespace Sportiva.Services;

public interface ICommentService
{
    // ════════════════════════════════════════════════════════════════
    //  Comments
    // ════════════════════════════════════════════════════════════════

    Task<Result<PostCommentResponse>> CreateCommentAsync(
        string userId, CreateCommentRequest request, CancellationToken ct = default);

    Task<Result> UpdateCommentAsync(
        string userId, string commentId, string content, CancellationToken ct = default);

    Task<Result> DeleteCommentAsync(
        string userId, string commentId, CancellationToken ct = default);

    Task<Result<PaginatedList<PostCommentResponse>>> GetPostCommentsAsync(
        string postId, string? currentUserId, RequestFilters filters, CancellationToken ct = default);

    Task<Result<ToggleCommentLikeResponse>> ToggleCommentLikeAsync(
        string userId, string commentId, CancellationToken ct = default);

    // ════════════════════════════════════════════════════════════════
    //  Replies
    // ════════════════════════════════════════════════════════════════

    Task<Result<CommentReplyResponse>> CreateReplyAsync(
        string userId, CreateReplyRequest request, CancellationToken ct = default);

    Task<Result> UpdateReplyAsync(
        string userId, string replyId, string content, CancellationToken ct = default);

    Task<Result> DeleteReplyAsync(
        string userId, string replyId, CancellationToken ct = default);

    Task<Result<PaginatedList<CommentReplyResponse>>> GetCommentRepliesAsync(
        string commentId, string? currentUserId, RequestFilters filters, CancellationToken ct = default);

    Task<Result<ToggleReplyLikeResponse>> ToggleReplyLikeAsync(
        string userId, string replyId, CancellationToken ct = default);
}
```

## File: Services/IPostService.cs
```csharp
using Sportiva.Contracts.Common;
using Sportiva.Contracts.Posts;

namespace Sportiva.Services;

public interface IPostService
{
    Task<Result<PostResponse>> CreatePostAsync(string userId, CreatePostRequest request, CancellationToken ct = default);
    Task<Result> SoftDeletePostAsync(string userId, string postId, CancellationToken ct = default);
    Task<Result> UpdatePostAsync(string userId, string postId, UpdatePostRequest request, CancellationToken ct = default);
    Task<Result<PostResponse>> GetPostAsync(string postId, string? currentUserId = null, CancellationToken ct = default);
    Task<PaginatedList<PostResponse>> GetPostsByUserAsync(string profileUserId, string? currentUserId, RequestFilters filters, CancellationToken ct = default);
    Task<PaginatedList<PostResponse>> GetPostsAsync(string? currentUserId, RequestFilters filters, CancellationToken ct = default);
    Task<Result<ToggleLikeResponse>> ToggleLikeAsync(string userId, string postId, CancellationToken ct = default);
    Task<Result<PaginatedList<PostLikerResponse>>> GetPostLikersAsync(string postId, RequestFilters filters, CancellationToken ct = default);
}
```
