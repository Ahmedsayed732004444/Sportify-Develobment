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