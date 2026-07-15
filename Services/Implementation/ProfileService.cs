using Sportiva.Contracts.Common;
using Sportiva.Contracts.Shared.Enums;
using Sportiva.Contracts.Shared.Summaries;
using Sportiva.Contracts.Users;
using Sportiva.Extensions;

namespace Sportiva.Services;

public class ProfileService(
    ApplicationDbContext context,
    ILogger<ProfileService> logger,
    IWebHostEnvironment env,
    IHttpContextAccessor accessor) : IProfileService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<ProfileService> _logger = logger;
    private readonly IWebHostEnvironment _env = env;
    private readonly IHttpContextAccessor _accessor = accessor;

    private static readonly string[] AllowedUserSortColumns = ["CreatedAt"];

    // ════════════════════════════════════════════════════════════════
    //  Get Profile
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<UserProfileResponse>> GetProfileAsync(
    string profileUserId, string? currentUserId = null, CancellationToken ct = default)
    {
        try
        {
            var raw = await _context.Users
                .Where(u => u.Id == profileUserId && !u.IsDisabled)
                .Select(u => new
                {
                    UserId = u.Id,
                    u.FirstName,
                    u.LastName,
                    u.FullName,
                    u.Email,
                    Bio = u.UserProfile == null ? null : u.UserProfile.Bio,
                    City = u.UserProfile == null ? null : u.UserProfile.City,
                    Country = u.UserProfile == null ? null : u.UserProfile.Country,
                    ProfilePictureUrl = u.UserProfile == null ? null : u.UserProfile.ProfilePictureUrl,
                    CoverImageUrl = u.UserProfile == null ? null : u.UserProfile.CoverImageUrl,
                    PreferredSport = u.UserProfile == null ? null : u.UserProfile.PreferredSport,
                    PreferredCity = u.UserProfile == null ? null : u.UserProfile.PreferredCity,
                    IsMe = u.Id == currentUserId,
                    IsFollowing = u.Followers.Any(f => f.FollowerId == currentUserId),
                    CanSendMessage = u.Id != currentUserId,
                    FollowersCount = u.Followers.Count,
                    FollowingCount = u.Following.Count,
                    PostsCount = u.Posts.Count(p => !p.IsDeleted),
                    u.CreatedAt
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (raw is null)
                return Result.Failure<UserProfileResponse>(UserErrors.UserNotFound);

            var response = new UserProfileResponse(
                raw.UserId,
                raw.FirstName,
                raw.LastName,
                raw.FullName,
                raw.Email!,
                raw.Bio,
                raw.City,
                raw.Country,
                raw.ProfilePictureUrl,
                raw.CoverImageUrl,
                raw.PreferredSport.HasValue ? (SportTypeDto?)raw.PreferredSport.Value : null,
                raw.PreferredCity,
                raw.IsMe,
                raw.IsFollowing,
                raw.CanSendMessage,
                raw.FollowersCount,
                raw.FollowingCount,
                raw.PostsCount,
                raw.CreatedAt
            );

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving profile for user {ProfileUserId}", profileUserId);
            return Result.Failure<UserProfileResponse>(ProfileErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Update Profile Info
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<UserProfileResponse>> UpdateProfileInfoAsync(
        string userId, UpdateProfileInfoRequest request, CancellationToken ct = default)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.UserProfile)
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDisabled, ct);

            if (user is null)
                return Result.Failure<UserProfileResponse>(UserErrors.UserNotFound);

            if (!string.IsNullOrWhiteSpace(request.FirstName))
                user.FirstName = request.FirstName.Trim();

            if (!string.IsNullOrWhiteSpace(request.LastName))
                user.LastName = request.LastName.Trim();

            if (user.UserProfile is null)
            {
                user.UserProfile = new UserProfile { UserId = userId };
                await _context.UserProfiles.AddAsync(user.UserProfile, ct);
            }

            var profile = user.UserProfile;

            if (request.Bio is not null) profile.Bio = request.Bio.Trim();
            if (request.City is not null) profile.City = request.City.Trim();
            if (request.Country is not null) profile.Country = request.Country.Trim();
            if (request.PreferredCity is not null) profile.PreferredCity = request.PreferredCity.Trim();
            if (request.PreferredSport.HasValue) profile.PreferredSport = (SportType)request.PreferredSport.Value;

            await _context.SaveChangesAsync(ct);

            return Result.Success(await BuildProfileResponseAsync(user, profile, userId, ct));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating profile info for user {UserId}", userId);
            return Result.Failure<UserProfileResponse>(ProfileErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Update Profile Photo
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<UserProfileResponse>> UpdateProfilePhotoAsync(
        string userId, UpdateProfilePhotoRequest request, CancellationToken ct = default)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.UserProfile)
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDisabled, ct);

            if (user is null)
                return Result.Failure<UserProfileResponse>(UserErrors.UserNotFound);

            if (user.UserProfile is null)
            {
                user.UserProfile = new UserProfile { UserId = userId };
                await _context.UserProfiles.AddAsync(user.UserProfile, ct);
            }

            var profile = user.UserProfile;
            var oldPicture = profile.ProfilePictureUrl;

            profile.ProfilePictureUrl = await FileHelper.UploadeFileAsync(
     request.ProfilePicture, "uploads/profiles", _env, _accessor);

            if (!string.IsNullOrEmpty(oldPicture))
                FileHelper.DeleteFile(oldPicture, "uploads/profiles", _env);

            await _context.SaveChangesAsync(ct);

            return Result.Success(await BuildProfileResponseAsync(user, profile, userId, ct));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating profile photo for user {UserId}", userId);
            return Result.Failure<UserProfileResponse>(ProfileErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Update Profile Cover
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<UserProfileResponse>> UpdateProfileCoverAsync(
        string userId, UpdateProfileCoverRequest request, CancellationToken ct = default)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.UserProfile)
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDisabled, ct);

            if (user is null)
                return Result.Failure<UserProfileResponse>(UserErrors.UserNotFound);

            if (user.UserProfile is null)
            {
                user.UserProfile = new UserProfile { UserId = userId };
                await _context.UserProfiles.AddAsync(user.UserProfile, ct);
            }

            var profile = user.UserProfile;
            var oldCover = profile.CoverImageUrl;

            profile.CoverImageUrl = await FileHelper.UploadeFileAsync(
    request.CoverImage, "uploads/covers", _env, _accessor);

            if (!string.IsNullOrEmpty(oldCover))
                FileHelper.DeleteFile(oldCover, "uploads/covers", _env);

            await _context.SaveChangesAsync(ct);

            return Result.Success(await BuildProfileResponseAsync(user, profile, userId, ct));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating profile cover for user {UserId}", userId);
            return Result.Failure<UserProfileResponse>(ProfileErrors.Error);
        }
    }

    // ── Private Helper ────────────────────────────────────────────
    private async Task<UserProfileResponse> BuildProfileResponseAsync(
        ApplicationUser user, UserProfile profile, string userId, CancellationToken ct)
    {
        return new UserProfileResponse(
            user.Id,
            user.FirstName,
            user.LastName,
            user.FullName,
            user.Email!,
            profile.Bio,
            profile.City,
            profile.Country,
            profile.ProfilePictureUrl,
            profile.CoverImageUrl,
            profile.PreferredSport.HasValue ? (SportTypeDto?)profile.PreferredSport.Value : null,
            profile.PreferredCity,
            IsMe: true,
            IsFollowing: false,
            CanSendMessage: false,
            FollowersCount: await _context.UserFollows.CountAsync(f => f.FollowingId == userId, ct),
            FollowingCount: await _context.UserFollows.CountAsync(f => f.FollowerId == userId, ct),
            PostsCount: await _context.Posts.CountAsync(p => p.UserId == userId && !p.IsDeleted, ct),
            user.CreatedAt
        );
    }

    // ════════════════════════════════════════════════════════════════
    //  Toggle Follow
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<ToggleFollowResponse>> ToggleFollowAsync(
        string currentUserId, string targetUserId, CancellationToken ct = default)
    {
        try
        {
            // مش منطقي تـ follow نفسك
            if (currentUserId == targetUserId)
                return Result.Failure<ToggleFollowResponse>(ProfileErrors.CannotFollowSelf);

            var targetExists = await _context.Users
                .AnyAsync(u => u.Id == targetUserId && !u.IsDisabled, ct);

            if (!targetExists)
                return Result.Failure<ToggleFollowResponse>(UserErrors.UserNotFound);

            var existingFollow = await _context.UserFollows
                .FirstOrDefaultAsync(f => f.FollowerId == currentUserId && f.FollowingId == targetUserId, ct);

            bool isNowFollowing;

            if (existingFollow is not null)
            {
                _context.UserFollows.Remove(existingFollow);
                isNowFollowing = false;
            }
            else
            {
                await _context.UserFollows.AddAsync(new UserFollow
                {
                    FollowerId = currentUserId,
                    FollowingId = targetUserId
                }, ct);
                isNowFollowing = true;
            }

            await _context.SaveChangesAsync(ct);

            var followersCount = await _context.UserFollows
                .CountAsync(f => f.FollowingId == targetUserId, ct);

            return Result.Success(new ToggleFollowResponse(targetUserId, isNowFollowing, followersCount));
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            _logger.LogWarning(
                "Duplicate follow attempt by {CurrentUserId} on {TargetUserId}",
                currentUserId, targetUserId);
            return Result.Failure<ToggleFollowResponse>(ProfileErrors.AlreadyFollowing);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occurred while toggling follow: follower={CurrentUserId}, target={TargetUserId}",
                currentUserId, targetUserId);
            return Result.Failure<ToggleFollowResponse>(ProfileErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Get Followers
    // ════════════════════════════════════════════════════════════════

    public async Task<PaginatedList<UserCardSummary>> GetFollowersAsync(
        string profileUserId, string? currentUserId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var query = _context.UserFollows
                .Where(f => f.FollowingId == profileUserId)
                .ApplyFilters(filters,
                    searchPredicate: f =>
                        f.Follower.FullName.Contains(filters.SearchValue!),
                    allowedSortColumns: ["FollowedAt"])
                .Select(f => new UserCardSummary(
                    f.FollowerId,
                    f.Follower.FullName,
                    f.Follower.UserProfile == null ? null : f.Follower.UserProfile.ProfilePictureUrl,
                    f.Follower.UserProfile == null ? null : f.Follower.UserProfile.Bio,
                    f.Follower.UserProfile == null ? null : f.Follower.UserProfile.City,
                    IsFollowing: _context.UserFollows
                        .Any(x => x.FollowerId == currentUserId && x.FollowingId == f.FollowerId),
                    IsMe: f.FollowerId == currentUserId,
                    FollowedAt: _context.UserFollows
                        .Where(x => x.FollowerId == currentUserId && x.FollowingId == f.FollowerId)
                        .Select(x => (DateTime?)x.FollowedAt)
                        .FirstOrDefault()
                ))
                .AsNoTracking();

            return await query.ToPaginatedListAsync(filters, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving followers for user {ProfileUserId}", profileUserId);
            throw;
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Get Following
    // ════════════════════════════════════════════════════════════════

    public async Task<PaginatedList<UserCardSummary>> GetFollowingAsync(
        string profileUserId, string? currentUserId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var query = _context.UserFollows
                .Where(f => f.FollowerId == profileUserId)
                .ApplyFilters(filters,
                    searchPredicate: f =>
                        f.Following.FullName.Contains(filters.SearchValue!),
                    allowedSortColumns: ["FollowedAt"])
                .Select(f => new UserCardSummary(
                    f.FollowingId,
                    f.Following.FullName,
                    f.Following.UserProfile == null ? null : f.Following.UserProfile.ProfilePictureUrl,
                    f.Following.UserProfile == null ? null : f.Following.UserProfile.Bio,
                    f.Following.UserProfile == null ? null : f.Following.UserProfile.City,
                    IsFollowing: _context.UserFollows
                        .Any(x => x.FollowerId == currentUserId && x.FollowingId == f.FollowingId),
                    IsMe: f.FollowingId == currentUserId,
                    FollowedAt: _context.UserFollows
                        .Where(x => x.FollowerId == currentUserId && x.FollowingId == f.FollowingId)
                        .Select(x => (DateTime?)x.FollowedAt)
                        .FirstOrDefault()
                ))
                .AsNoTracking();

            return await query.ToPaginatedListAsync(filters, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving following for user {ProfileUserId}", profileUserId);
            throw;
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Search Users
    // ════════════════════════════════════════════════════════════════

    public async Task<PaginatedList<UserCardSummary>> SearchUsersAsync(
        string? currentUserId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var query = _context.Users
                .Where(u => !u.IsDisabled)
                .ApplyFilters(filters,
                    searchPredicate: u =>
                        u.FullName.Contains(filters.SearchValue!) ||
                        (u.UserProfile != null && u.UserProfile.City!.Contains(filters.SearchValue!)),
                    allowedSortColumns: AllowedUserSortColumns)
                .Select(u => new UserCardSummary(
                    u.Id,
                    u.FullName,
                    u.UserProfile == null ? null : u.UserProfile.ProfilePictureUrl,
                    u.UserProfile == null ? null : u.UserProfile.Bio,
                    u.UserProfile == null ? null : u.UserProfile.City,
                    IsFollowing: u.Followers.Any(f => f.FollowerId == currentUserId),
                    IsMe: u.Id == currentUserId,
                    FollowedAt: u.Followers
                        .Where(f => f.FollowerId == currentUserId)
                        .Select(f => (DateTime?)f.FollowedAt)
                        .FirstOrDefault()
                ))
                .AsNoTracking();

            return await query.ToPaginatedListAsync(filters, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching users");
            throw;
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Private Helpers
    // ════════════════════════════════════════════════════════════════

    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        var message = ex.InnerException?.Message ?? ex.Message;
        return message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase)
            || message.Contains("duplicate", StringComparison.OrdinalIgnoreCase)
            || message.Contains("IX_", StringComparison.OrdinalIgnoreCase);
    }
}