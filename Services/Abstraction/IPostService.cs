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