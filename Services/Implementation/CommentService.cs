using Sportiva.Contracts.Common;
using Sportiva.Contracts.Posts;
using Sportiva.Contracts.Shared.Summaries;
using Sportiva.Extensions;

namespace Sportiva.Services;

public class CommentService(
    ApplicationDbContext context,
    ILogger<CommentService> logger) : ICommentService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<CommentService> _logger = logger;

    private static readonly string[] AllowedCommentSortColumns = ["CreatedAt", "LikesCount"];
    private static readonly string[] AllowedReplySortColumns = ["CreatedAt", "LikesCount"];

    // ════════════════════════════════════════════════════════════════
    //  Comments
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<PostCommentResponse>> CreateCommentAsync(
        string userId, CreateCommentRequest request, CancellationToken ct = default)
    {
        try
        {
            var postExists = await _context.Posts
                .AnyAsync(p => p.Id == request.PostId && !p.IsDeleted, ct);

            if (!postExists)
                return Result.Failure<PostCommentResponse>(PostErrors.PostNotFound);

            var author = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new
                {
                    u.FullName,
                    ProfilePictureUrl = u.UserProfile == null
                        ? null
                        : u.UserProfile.ProfilePictureUrl
                })
                .FirstOrDefaultAsync(ct);

            if (author is null)
                return Result.Failure<PostCommentResponse>(UserErrors.UserNotFound);

            var comment = new PostComment
            {
                PostId = request.PostId,
                UserId = userId,
                Content = request.Content
            };

            await _context.PostComments.AddAsync(comment, ct);
            await _context.SaveChangesAsync(ct);

            var response = new PostCommentResponse(
                comment.Id,
                comment.PostId,
                comment.Content,
                new UserSummary(userId, author.FullName, author.ProfilePictureUrl),
                IsOwner: true,
                ILiked: false,
                LikesCount: 0,
                RepliesCount: 0,
                comment.CreatedAt
            );

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occurred while creating comment on post {PostId} for user {UserId}",
                request.PostId, userId);
            return Result.Failure<PostCommentResponse>(PostErrors.Error);
        }
    }

    public async Task<Result> UpdateCommentAsync(
        string userId, string commentId, string content, CancellationToken ct = default)
    {
        try
        {
            var comment = await _context.PostComments
                .FirstOrDefaultAsync(c => c.Id == commentId && c.UserId == userId && !c.IsDeleted, ct);

            if (comment is null)
                return Result.Failure(CommentErrors.CommentNotFound);

            comment.Content = content;
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occurred while updating comment {CommentId} for user {UserId}",
                commentId, userId);
            return Result.Failure(CommentErrors.Error);
        }
    }

    public async Task<Result> DeleteCommentAsync(
        string userId, string commentId, CancellationToken ct = default)
    {
        try
        {
            var comment = await _context.PostComments
                .FirstOrDefaultAsync(c => c.Id == commentId && c.UserId == userId && !c.IsDeleted, ct);

            if (comment is null)
                return Result.Failure(CommentErrors.CommentNotFound);

            comment.IsDeleted = true;
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occurred while deleting comment {CommentId} for user {UserId}",
                commentId, userId);
            return Result.Failure(CommentErrors.Error);
        }
    }

    public async Task<Result<PaginatedList<PostCommentResponse>>> GetPostCommentsAsync(
        string postId, string? currentUserId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var postExists = await _context.Posts
                .AnyAsync(p => p.Id == postId && !p.IsDeleted, ct);

            if (!postExists)
                return Result.Failure<PaginatedList<PostCommentResponse>>(PostErrors.PostNotFound);

            var query = _context.PostComments
                .Where(c => c.PostId == postId && !c.IsDeleted)
                .ApplyFilters(filters,
                    searchPredicate: x => x.Content != null && x.Content.Contains(filters.SearchValue!),
                    allowedSortColumns: AllowedCommentSortColumns)
                .Select(c => new PostCommentResponse(
                    c.Id,
                    c.PostId,
                    c.Content,
                    new UserSummary(
                        c.UserId,
                        c.User.FullName,
                        c.User.UserProfile == null ? null : c.User.UserProfile.ProfilePictureUrl),
                    IsOwner: c.UserId == currentUserId,
                    ILiked: c.Reactions.Any(r => r.UserId == currentUserId),
                    LikesCount: c.Reactions.Count,
                    RepliesCount: c.Replies.Count(r => !r.IsDeleted),
                    c.CreatedAt
                ))
                .AsNoTracking();

            var result = await query.ToPaginatedListAsync(filters, ct);
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving comments for post {PostId}", postId);
            return Result.Failure<PaginatedList<PostCommentResponse>>(PostErrors.Error);
        }
    }

    public async Task<Result<ToggleCommentLikeResponse>> ToggleCommentLikeAsync(
        string userId, string commentId, CancellationToken ct = default)
    {
        try
        {
            var commentExists = await _context.PostComments
                .AnyAsync(c => c.Id == commentId && !c.IsDeleted, ct);

            if (!commentExists)
                return Result.Failure<ToggleCommentLikeResponse>(CommentErrors.CommentNotFound);

            var existingReaction = await _context.CommentReactions
                .FirstOrDefaultAsync(r => r.CommentId == commentId && r.UserId == userId, ct);

            bool isNowLiked;

            if (existingReaction is not null)
            {
                _context.CommentReactions.Remove(existingReaction);
                isNowLiked = false;
            }
            else
            {
                await _context.CommentReactions.AddAsync(new CommentReaction
                {
                    CommentId = commentId,
                    UserId = userId
                }, ct);
                isNowLiked = true;
            }

            await _context.SaveChangesAsync(ct);

            var likesCount = await _context.CommentReactions
                .CountAsync(r => r.CommentId == commentId, ct);

            return Result.Success(new ToggleCommentLikeResponse(commentId, isNowLiked, likesCount));
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            _logger.LogWarning("Duplicate reaction attempt on comment {CommentId} by user {UserId}", commentId, userId);
            return Result.Failure<ToggleCommentLikeResponse>(CommentErrors.AlreadyLiked);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occurred while toggling like on comment {CommentId} for user {UserId}",
                commentId, userId);
            return Result.Failure<ToggleCommentLikeResponse>(CommentErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Replies
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<CommentReplyResponse>> CreateReplyAsync(
        string userId, CreateReplyRequest request, CancellationToken ct = default)
    {
        try
        {
            var commentExists = await _context.PostComments
                .AnyAsync(c => c.Id == request.CommentId && !c.IsDeleted, ct);

            if (!commentExists)
                return Result.Failure<CommentReplyResponse>(CommentErrors.CommentNotFound);

            var author = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new
                {
                    u.FullName,
                    ProfilePictureUrl = u.UserProfile == null
                        ? null
                        : u.UserProfile.ProfilePictureUrl
                })
                .FirstOrDefaultAsync(ct);

            if (author is null)
                return Result.Failure<CommentReplyResponse>(UserErrors.UserNotFound);

            var reply = new CommentReply
            {
                CommentId = request.CommentId,
                UserId = userId,
                Content = request.Content
            };

            await _context.CommentReplies.AddAsync(reply, ct);
            await _context.SaveChangesAsync(ct);

            var response = new CommentReplyResponse(
                reply.Id,
                reply.CommentId,
                reply.Content,
                new UserSummary(userId, author.FullName, author.ProfilePictureUrl),
                IsOwner: true,
                ILiked: false,
                LikesCount: 0,
                reply.CreatedAt
            );

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occurred while creating reply on comment {CommentId} for user {UserId}",
                request.CommentId, userId);
            return Result.Failure<CommentReplyResponse>(ReplyErrors.Error);
        }
    }

    public async Task<Result> UpdateReplyAsync(
        string userId, string replyId, string content, CancellationToken ct = default)
    {
        try
        {
            var reply = await _context.CommentReplies
                .FirstOrDefaultAsync(r => r.Id == replyId && r.UserId == userId && !r.IsDeleted, ct);

            if (reply is null)
                return Result.Failure(ReplyErrors.ReplyNotFound);

            reply.Content = content;
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occurred while updating reply {ReplyId} for user {UserId}",
                replyId, userId);
            return Result.Failure(ReplyErrors.Error);
        }
    }

    public async Task<Result> DeleteReplyAsync(
        string userId, string replyId, CancellationToken ct = default)
    {
        try
        {
            var reply = await _context.CommentReplies
                .FirstOrDefaultAsync(r => r.Id == replyId && r.UserId == userId && !r.IsDeleted, ct);

            if (reply is null)
                return Result.Failure(ReplyErrors.ReplyNotFound);

            reply.IsDeleted = true;
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occurred while deleting reply {ReplyId} for user {UserId}",
                replyId, userId);
            return Result.Failure(ReplyErrors.Error);
        }
    }

    public async Task<Result<PaginatedList<CommentReplyResponse>>> GetCommentRepliesAsync(
        string commentId, string? currentUserId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var commentExists = await _context.PostComments
                .AnyAsync(c => c.Id == commentId && !c.IsDeleted, ct);

            if (!commentExists)
                return Result.Failure<PaginatedList<CommentReplyResponse>>(CommentErrors.CommentNotFound);

            var query = _context.CommentReplies
                .Where(r => r.CommentId == commentId && !r.IsDeleted)
                .ApplyFilters(filters,
                    searchPredicate: x => x.Content != null && x.Content.Contains(filters.SearchValue!),
                    allowedSortColumns: AllowedReplySortColumns)
                .Select(r => new CommentReplyResponse(
                    r.Id,
                    r.CommentId,
                    r.Content,
                    new UserSummary(
                        r.UserId,
                        r.User.FullName,
                        r.User.UserProfile == null ? null : r.User.UserProfile.ProfilePictureUrl),
                    IsOwner: r.UserId == currentUserId,
                    ILiked: r.Reactions.Any(x => x.UserId == currentUserId),
                    LikesCount: r.Reactions.Count,
                    r.CreatedAt
                ))
                .AsNoTracking();

            var result = await query.ToPaginatedListAsync(filters, ct);
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving replies for comment {CommentId}", commentId);
            return Result.Failure<PaginatedList<CommentReplyResponse>>(CommentErrors.Error);
        }
    }

    public async Task<Result<ToggleReplyLikeResponse>> ToggleReplyLikeAsync(
        string userId, string replyId, CancellationToken ct = default)
    {
        try
        {
            var replyExists = await _context.CommentReplies
                .AnyAsync(r => r.Id == replyId && !r.IsDeleted, ct);

            if (!replyExists)
                return Result.Failure<ToggleReplyLikeResponse>(ReplyErrors.ReplyNotFound);

            var existingReaction = await _context.ReplyReactions
                .FirstOrDefaultAsync(r => r.ReplyId == replyId && r.UserId == userId, ct);

            bool isNowLiked;

            if (existingReaction is not null)
            {
                _context.ReplyReactions.Remove(existingReaction);
                isNowLiked = false;
            }
            else
            {
                await _context.ReplyReactions.AddAsync(new ReplyReaction
                {
                    ReplyId = replyId,
                    UserId = userId
                }, ct);
                isNowLiked = true;
            }

            await _context.SaveChangesAsync(ct);

            var likesCount = await _context.ReplyReactions
                .CountAsync(r => r.ReplyId == replyId, ct);

            return Result.Success(new ToggleReplyLikeResponse(replyId, isNowLiked, likesCount));
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            _logger.LogWarning("Duplicate reaction attempt on reply {ReplyId} by user {UserId}", replyId, userId);
            return Result.Failure<ToggleReplyLikeResponse>(ReplyErrors.AlreadyLiked);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occurred while toggling like on reply {ReplyId} for user {UserId}",
                replyId, userId);
            return Result.Failure<ToggleReplyLikeResponse>(ReplyErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Private Helpers
    // ════════════════════════════════════════════════════════════════

    /// <summary>
    /// يتحقق إن الـ Exception سببه Unique Constraint Violation
    /// يشتغل مع SQL Server و SQLite
    /// </summary>
    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        var message = ex.InnerException?.Message ?? ex.Message;
        return message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase)
            || message.Contains("duplicate", StringComparison.OrdinalIgnoreCase)
            || message.Contains("IX_", StringComparison.OrdinalIgnoreCase);
    }
}