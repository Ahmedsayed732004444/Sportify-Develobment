This file is a merged representation of a subset of the codebase, containing files not matching ignore patterns, combined into a single document by Repomix.

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
- Files matching these patterns are excluded: ai-context.md, repomix-output.xml, keys/**, wwwroot/**, **/*.xml, **/*.csproj, **/*.sln, **/*.user, **/*.designer.cs, **/*.g.cs, **/bin/**, **/obj/**, **/.vs/**, **/Migrations/**
- Files matching patterns in .gitignore are excluded
- Files matching default ignore patterns are excluded
- Files are sorted by Git change count (files with more changes are at the bottom)

# Directory Structure
```
.gitignore
Abstractions/Consts/DefaultRoles.cs
Abstractions/Consts/DefaultUsers.cs
Abstractions/Consts/Permissions.cs
Abstractions/Consts/RegexPatterns.cs
Abstractions/Error.cs
Abstractions/PaginatedList.cs
Abstractions/Result.cs
Abstractions/ResultExtensions.cs
appsettings.json
Authentication/Filters/HasPermissionAttribute.cs
Authentication/Filters/PermissionAuthorizationHandler.cs
Authentication/Filters/PermissionAuthorizationPolicyProvider.cs
Authentication/Filters/PermissionRequirement.cs
Authentication/IJwtProvider.cs
Authentication/JwtOptions.cs
Authentication/JwtProvider.cs
CancellationExceptionFilter.cs
Contracts/Authentication/AuthResponse.cs
Contracts/Authentication/ConfirmEmailRequest.cs
Contracts/Authentication/ConfirmEmailRequestValidator.cs
Contracts/Authentication/ForgetPasswordRequest.cs
Contracts/Authentication/ForgetPasswordRequestValidator.cs
Contracts/Authentication/LoginRequest.cs
Contracts/Authentication/LoginRequestValidator.cs
Contracts/Authentication/RefreshTokenRequest.cs
Contracts/Authentication/RefreshTokenRequestValidator.cs
Contracts/Authentication/RegisterRequest.cs
Contracts/Authentication/RegisterRequestValidator.cs
Contracts/Authentication/ResendConfirmationEmailRequest.cs
Contracts/Authentication/ResendConfirmationEmailRequestValidator.cs
Contracts/Authentication/ResetPasswordRequest.cs
Contracts/Authentication/ResetPasswordRequestValidator.cs
Contracts/Bookings/BookingResponse.cs
Contracts/Bookings/CreateBookingRequest.cs
Contracts/Clubs/ClubResponse.cs
Contracts/Clubs/ClubSubscriptionSummary.cs
Contracts/Clubs/CreateClubRequest.cs
Contracts/Clubs/UpdateClubRequest.cs
Contracts/Common/RequestFilters.cs
Contracts/Courts/CourtResponse.cs
Contracts/Courts/CreateCourtRequest.cs
Contracts/Courts/UpdateCourtRequest.cs
Contracts/Matches/CreateFriendlyMatchRequest.cs
Contracts/Matches/FriendlyMatchResponse.cs
Contracts/Matches/JoinMatchRequest.cs
Contracts/Matches/MatchJoinRequestResponse.cs
Contracts/Matches/ReviewJoinRequestRequest.cs
Contracts/Memberships/CreateMembershipUpgradeRequest.cs
Contracts/Memberships/MembershipUpgradeResponse.cs
Contracts/Memberships/ReviewMembershipUpgradeRequest.cs
Contracts/Messaging/ConversationSummary.cs
Contracts/Messaging/MessageResponse.cs
Contracts/Messaging/SendMessageRequest.cs
Contracts/Notifications/BulkUpdateNotificationPreferencesRequest.cs
Contracts/Notifications/NotificationCountersResponse.cs
Contracts/Notifications/NotificationListResponse.cs
Contracts/Notifications/NotificationPreferenceItem.cs
Contracts/Notifications/NotificationPreferenceResponse.cs
Contracts/Notifications/NotificationPreferencesListResponse.cs
Contracts/Notifications/NotificationResponse.cs
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
Contracts/Reviews/CreateReviewRequest.cs
Contracts/Reviews/ReviewResponse.cs
Contracts/Shared/Enums/BookingStatusDto.cs
Contracts/Shared/Enums/JoinRequestStatusDto.cs
Contracts/Shared/Enums/MatchStatusDto.cs
Contracts/Shared/Enums/NotificationPriorityDto.cs
Contracts/Shared/Enums/NotificationTypeDto.cs
Contracts/Shared/Enums/PaymentStatusDto.cs
Contracts/Shared/Enums/RequestStatusDto.cs
Contracts/Shared/Enums/SportTypeDto.cs
Contracts/Shared/Enums/TournamentStatusDto.cs
Contracts/Shared/Summaries/ClubSummary.cs
Contracts/Shared/Summaries/CourtSummary.cs
Contracts/Shared/Summaries/FriendlyMatchSummary.cs
Contracts/Shared/Summaries/ParticipantSummary.cs
Contracts/Shared/Summaries/ReviewSummary.cs
Contracts/Shared/Summaries/SubscriptionPlanSummary.cs
Contracts/Shared/Summaries/TimeSlotSummary.cs
Contracts/Shared/Summaries/UserCardSummary.cs
Contracts/Shared/Summaries/UserSummary.cs
Contracts/Subscriptions/ClubSubscriptionResponse.cs
Contracts/Subscriptions/CreateClubSubscriptionRequest.cs
Contracts/Subscriptions/SubscriptionPaymentSummary.cs
Contracts/Subscriptions/SubscriptionPlanResponse.cs
Contracts/TimeSlots/CreateTimeSlotRequest.cs
Contracts/TimeSlots/TimeSlotResponse.cs
Contracts/Tournaments/CreateTournamentMatchRequest.cs
Contracts/Tournaments/CreateTournamentRequest.cs
Contracts/Tournaments/SetTournamentMatchWinnerRequest.cs
Contracts/Tournaments/TournamentMatchResponse.cs
Contracts/Tournaments/TournamentResponse.cs
Contracts/Users/ToggleFollowResponse.cs
Contracts/Users/UpdateProfileCoverRequest.cs
Contracts/Users/UpdateProfileInfoRequest.cs
Contracts/Users/UpdateProfilePhotoRequest.cs
Contracts/Users/UserProfileResponse.cs
Controllers/AuthController.cs
Controllers/ClubsController.cs
Controllers/CommentsController.cs
Controllers/PostsController.cs
Controllers/ProfilesController.cs
DependencyInjection.cs
Entities/ApplicationRole.cs
Entities/ApplicationUser.cs
Entities/Booking.cs
Entities/Club.cs
Entities/ClubSubscription.cs
Entities/CommentReaction.cs
Entities/CommentReply.cs
Entities/Court.cs
Entities/FriendlyMatch.cs
Entities/MatchJoinRequest.cs
Entities/MembershipUpgrade.cs
Entities/Message.cs
Entities/Notification.cs
Entities/NotificationPreference.cs
Entities/Post.cs
Entities/PostComment.cs
Entities/PostLike.cs
Entities/RefreshToken.cs
Entities/ReplyReaction.cs
Entities/Review.cs
Entities/SubscriptionPayment.cs
Entities/SubscriptionPlan.cs
Entities/TimeSlot.cs
Entities/Tournament.cs
Entities/TournamentMatch.cs
Entities/TournamentParticipant.cs
Entities/UserFollow.cs
Entities/UserProfile.cs
Enums/BookingStatus.cs
Enums/JoinRequestStatus.cs
Enums/MatchStatus.cs
Enums/NotificationPriority.cs
Enums/NotificationType.cs
Enums/PaymentStatus.cs
Enums/RequestStatus.cs
Enums/SportType.cs
Errors/ClubErrors.cs
Errors/CommentErrors.cs
Errors/PostErrors.cs
Errors/ProfileErrors.cs
Errors/UserErrors.cs
Extensions/QueryableExtensions.cs
Extensions/UserExtensions.cs
GlobalUsings.cs
Helpers/EmailBodyBuilder.cs
Helpers/FileHelper.cs
Helpers/ForgetPasswordBodyBuilder.cs
Mapping/MappingConfigurations.cs
Persistence/ApplicationDbContext.cs
Persistence/EntitiesConfigurations/BookingConfiguration.cs
Persistence/EntitiesConfigurations/ClubConfiguration.cs
Persistence/EntitiesConfigurations/ClubSubscriptionConfiguration.cs
Persistence/EntitiesConfigurations/CourtConfiguration.cs
Persistence/EntitiesConfigurations/DefaultRoles.cs
Persistence/EntitiesConfigurations/FriendlyMatchConfiguration.cs
Persistence/EntitiesConfigurations/MatchJoinRequestConfiguration.cs
Persistence/EntitiesConfigurations/MembershipUpgradeConfiguration.cs
Persistence/EntitiesConfigurations/PostConfiguration.cs
Persistence/EntitiesConfigurations/PostLikeConfiguration.cs
Persistence/EntitiesConfigurations/ReviewConfiguration.cs
Persistence/EntitiesConfigurations/RoleClaimConfiguration.cs
Persistence/EntitiesConfigurations/RoleConfiguration.cs
Persistence/EntitiesConfigurations/SubscriptionPaymentConfiguration.cs
Persistence/EntitiesConfigurations/SubscriptionPlanConfiguration.cs
Persistence/EntitiesConfigurations/TimeSlotConfiguration.cs
Persistence/EntitiesConfigurations/UserConfiguration.cs
Persistence/EntitiesConfigurations/UserProfileConfiguration.cs
Persistence/EntitiesConfigurations/UserRoleConfiguration.cs
Program.cs
Properties/launchSettings.json
README.md
repomix.config.json
Services/Abstraction/IAuthService.cs
Services/Abstraction/IBookingService.cs
Services/Abstraction/IClubService.cs
Services/Abstraction/IClubSubscriptionService.cs
Services/Abstraction/ICommentService.cs
Services/Abstraction/ICourtService.cs
Services/Abstraction/IFriendlyMatchService.cs
Services/Abstraction/IMatchJoinRequestService.cs
Services/Abstraction/IMembershipUpgradeService.cs
Services/Abstraction/IMessagingService.cs
Services/Abstraction/INotificationService.cs
Services/Abstraction/IPostService.cs
Services/Abstraction/IProfileService.cs
Services/Abstraction/IReviewService.cs
Services/Abstraction/ISubscriptionPlanService.cs
Services/Abstraction/ITimeSlotService.cs
Services/Abstraction/ITournamentService.cs
Services/Implementation/AuthService.cs
Services/Implementation/ClubService.cs
Services/Implementation/CommentService.cs
Services/Implementation/EmailService.cs
Services/Implementation/PostService.cs
Services/Implementation/ProfileService.cs
Settings/GitHubOAuthOptions.cs
Settings/GoogleOAuthOptions.cs
Settings/MailSettings.cs
sportiva-api-reference.html
tree.txt
```

# Files

## File: Controllers/ClubsController.cs
```csharp
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
```

## File: Errors/ClubErrors.cs
```csharp
namespace Sportiva.Errors;

public record ClubErrors
{
    public static readonly Error Error =
        new("Clubs.Error", "An error occurred while processing the club", StatusCodes.Status500InternalServerError);

    public static readonly Error ClubNotFound =
        new("Clubs.NotFound", "The specified club was not found", StatusCodes.Status404NotFound);

    public static readonly Error Unauthorized =
        new("Clubs.Unauthorized", "You are not authorized to manage this club", StatusCodes.Status403Forbidden);
}
```

## File: Persistence/EntitiesConfigurations/UserRoleConfiguration.cs
```csharp
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sportiva.Persistence.EntitiesConfigurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {
        builder.HasData(new IdentityUserRole<string>
        {
            UserId = DefaultUsers.Admin.Id,
            RoleId = DefaultRoles.Admin.Id
        });
    }
}
```

## File: Services/Implementation/ClubService.cs
```csharp
using Sportiva.Contracts.Clubs;
using Sportiva.Contracts.Common;
using Sportiva.Contracts.Shared.Summaries;
using Sportiva.Extensions;

namespace Sportiva.Services;

public class ClubService(
    ApplicationDbContext context,
    ILogger<ClubService> logger,
    IWebHostEnvironment env,
    IHttpContextAccessor accessor) : IClubService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<ClubService> _logger = logger;
    private readonly IWebHostEnvironment _env = env;
    private readonly IHttpContextAccessor _accessor = accessor;

    private static readonly string[] AllowedClubSortColumns = ["Name", "CreatedAt"];
    private const string LogoLocation = "uploads/clubs";

    // ════════════════════════════════════════════════════════════════
    //  Get Single Club
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<ClubResponse>> GetClubAsync(
        string clubId, string? currentUserId = null, CancellationToken ct = default)
    {
        try
        {
            var club = await _context.Clubs
                .Where(c => c.Id == clubId && !c.IsDeleted)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.LogoUrl,
                    c.Governorate,
                    c.City,
                    c.Address,
                    c.PhoneNumber,
                    c.Email,
                    c.IsActive,
                    c.OwnerId,
                    OwnerFullName = c.Owner.FullName,
                    OwnerPicture = c.Owner.UserProfile == null ? null : c.Owner.UserProfile.ProfilePictureUrl,
                    CourtsCount = c.Courts.Count(x => !x.IsDeleted),
                    c.CreatedAt,
                    ActiveSubscription = c.Subscriptions
                        .Where(s => !s.IsDeleted &&
                                    s.StartDate <= DateTime.UtcNow &&
                                    s.EndDate >= DateTime.UtcNow)
                        .Select(s => new
                        {
                            s.Id,
                            s.StartDate,
                            s.EndDate,
                            s.PlanId,
                            PlanName = s.Plan.Name,
                            PlanPrice = s.Plan.MonthlyPrice,
                            PlanMaxCourts = s.Plan.MaxCourts
                        })
                        .FirstOrDefault()
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (club is null)
                return Result.Failure<ClubResponse>(ClubErrors.ClubNotFound);

            var reviewRatings = await _context.Reviews
                .Where(r => r.Court.ClubId == clubId && !r.IsDeleted)
                .Select(r => r.Rating)
                .ToListAsync(ct);

            var isOwner = club.OwnerId == currentUserId;

            var response = new ClubResponse(
                club.Id,
                club.Name,
                club.LogoUrl,
                club.Governorate,
                club.City,
                club.Address,
                club.PhoneNumber,
                club.Email,
                club.IsActive,
                new UserSummary(club.OwnerId, club.OwnerFullName, club.OwnerPicture),
                IsOwner: isOwner,
                CanManageCourts: isOwner,
                CourtsCount: club.CourtsCount,
                ReviewsCount: reviewRatings.Count,
                AverageRating: reviewRatings.Count == 0 ? 0 : Math.Round(reviewRatings.Average(), 1),
                ActiveSubscription: club.ActiveSubscription is null
                    ? null
                    : new ClubSubscriptionSummary(
                        club.ActiveSubscription.Id,
                        new SubscriptionPlanSummary(
                            club.ActiveSubscription.PlanId,
                            club.ActiveSubscription.PlanName,
                            club.ActiveSubscription.PlanPrice,
                            club.ActiveSubscription.PlanMaxCourts),
                        club.ActiveSubscription.StartDate,
                        club.ActiveSubscription.EndDate,
                        IsActive: true),
                club.CreatedAt
            );

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving club {ClubId}", clubId);
            return Result.Failure<ClubResponse>(ClubErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Browse Clubs (public discovery — active only)
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<PaginatedList<ClubResponse>>> GetClubsAsync(
    string? currentUserId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var reviewStats = _context.Reviews
                .Where(r => !r.IsDeleted)
                .GroupBy(r => r.Court.ClubId)
                .Select(g => new
                {
                    ClubId = g.Key,
                    Count = (int?)g.Count(),
                    Average = (double?)g.Average(r => (double)r.Rating)
                });

            var clubsQuery = _context.Clubs
                .Where(c => !c.IsDeleted && c.IsActive)
                .ApplyFilters(filters,
                    searchPredicate: c =>
                        (c.Name != null && c.Name.Contains(filters.SearchValue!)) ||
                        (c.City != null && c.City.Contains(filters.SearchValue!)) ||
                        (c.Governorate != null && c.Governorate.Contains(filters.SearchValue!)),
                    allowedSortColumns: AllowedClubSortColumns);

            var query =
                from c in clubsQuery
                join rs in reviewStats on c.Id equals rs.ClubId into ratingsGroup
                from rs in ratingsGroup.DefaultIfEmpty()
                select new ClubResponse(
                    c.Id,
                    c.Name,
                    c.LogoUrl,
                    c.Governorate,
                    c.City,
                    c.Address,
                    c.PhoneNumber,
                    c.Email,
                    c.IsActive,
                    new UserSummary(
                        c.OwnerId,
                        c.Owner.FullName,
                        c.Owner.UserProfile == null ? null : c.Owner.UserProfile.ProfilePictureUrl),
                    IsOwner: c.OwnerId == currentUserId,
                    CanManageCourts: c.OwnerId == currentUserId,
                    CourtsCount: c.Courts.Count(x => !x.IsDeleted),
                    ReviewsCount: rs.Count ?? 0,
                    AverageRating: rs.Average ?? 0,
                    ActiveSubscription: c.Subscriptions
                        .Where(s => !s.IsDeleted &&
                                    s.StartDate <= DateTime.UtcNow &&
                                    s.EndDate >= DateTime.UtcNow)
                        .Select(s => new ClubSubscriptionSummary(
                            s.Id,
                            new SubscriptionPlanSummary(s.PlanId, s.Plan.Name, s.Plan.MonthlyPrice, s.Plan.MaxCourts),
                            s.StartDate,
                            s.EndDate,
                            true))
                        .FirstOrDefault(),
                    c.CreatedAt
                );

            var result = await query.AsNoTracking().ToPaginatedListAsync(filters, ct);
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving clubs");
            return Result.Failure<PaginatedList<ClubResponse>>(ClubErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Create Club
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<ClubResponse>> CreateClubAsync(
        string ownerId, CreateClubRequest request, CancellationToken ct = default)
    {
        try
        {
            var owner = await _context.Users
                .Where(u => u.Id == ownerId)
                .Select(u => new
                {
                    u.FullName,
                    ProfilePictureUrl = u.UserProfile == null ? null : u.UserProfile.ProfilePictureUrl
                })
                .FirstOrDefaultAsync(ct);

            if (owner is null)
                return Result.Failure<ClubResponse>(UserErrors.UserNotFound);

            var club = new Club
            {
                OwnerId = ownerId,
                Name = request.Name,
                Governorate = request.Governorate,
                City = request.City,
                Address = request.Address,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                IsActive = true
            };

            if (request.Logo is not null)
                club.LogoUrl = await FileHelper.UploadeFileAsync(request.Logo, LogoLocation, _env, _accessor);

            await _context.Clubs.AddAsync(club, ct);
            await _context.SaveChangesAsync(ct);

            var response = new ClubResponse(
                club.Id,
                club.Name,
                club.LogoUrl,
                club.Governorate,
                club.City,
                club.Address,
                club.PhoneNumber,
                club.Email,
                club.IsActive,
                new UserSummary(ownerId, owner.FullName, owner.ProfilePictureUrl),
                IsOwner: true,
                CanManageCourts: true,
                CourtsCount: 0,
                ReviewsCount: 0,
                AverageRating: 0,
                ActiveSubscription: null,
                club.CreatedAt
            );

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating club for owner {OwnerId}", ownerId);
            return Result.Failure<ClubResponse>(ClubErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Update Club
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<ClubResponse>> UpdateClubAsync(
        string userId, string clubId, UpdateClubRequest request, CancellationToken ct = default)
    {
        try
        {
            var club = await _context.Clubs
                .Include(c => c.Owner)
                    .ThenInclude(o => o.UserProfile)
                .FirstOrDefaultAsync(c => c.Id == clubId && !c.IsDeleted, ct);

            if (club is null)
                return Result.Failure<ClubResponse>(ClubErrors.ClubNotFound);

            if (club.OwnerId != userId)
                return Result.Failure<ClubResponse>(ClubErrors.Unauthorized);

            if (request.Name is not null) club.Name = request.Name;
            if (request.Governorate is not null) club.Governorate = request.Governorate;
            if (request.City is not null) club.City = request.City;
            if (request.Address is not null) club.Address = request.Address;
            if (request.PhoneNumber is not null) club.PhoneNumber = request.PhoneNumber;
            if (request.Email is not null) club.Email = request.Email;
            club.IsActive = request.IsActive;

            if (request.Logo is not null)
            {
                var oldLogo = club.LogoUrl;
                club.LogoUrl = await FileHelper.UploadeFileAsync(request.Logo, LogoLocation, _env, _accessor);

                if (!string.IsNullOrEmpty(oldLogo))
                    FileHelper.DeleteFile(oldLogo, LogoLocation, _env);
            }

            await _context.SaveChangesAsync(ct);

            var courtsCount = await _context.Courts
                .CountAsync(x => x.ClubId == clubId && !x.IsDeleted, ct);

            var reviewRatings = await _context.Reviews
                .Where(r => r.Court.ClubId == clubId && !r.IsDeleted)
                .Select(r => r.Rating)
                .ToListAsync(ct);

            var activeSub = await _context.ClubSubscriptions
                .Where(s => s.ClubId == clubId && !s.IsDeleted &&
                            s.StartDate <= DateTime.UtcNow && s.EndDate >= DateTime.UtcNow)
                .Select(s => new ClubSubscriptionSummary(
                    s.Id,
                    new SubscriptionPlanSummary(s.PlanId, s.Plan.Name, s.Plan.MonthlyPrice, s.Plan.MaxCourts),
                    s.StartDate,
                    s.EndDate,
                    true))
                .FirstOrDefaultAsync(ct);

            var response = new ClubResponse(
                club.Id,
                club.Name,
                club.LogoUrl,
                club.Governorate,
                club.City,
                club.Address,
                club.PhoneNumber,
                club.Email,
                club.IsActive,
                new UserSummary(
                    club.OwnerId,
                    club.Owner.FullName,
                    club.Owner.UserProfile == null ? null : club.Owner.UserProfile.ProfilePictureUrl),
                IsOwner: true,
                CanManageCourts: true,
                CourtsCount: courtsCount,
                ReviewsCount: reviewRatings.Count,
                AverageRating: reviewRatings.Count == 0 ? 0 : Math.Round(reviewRatings.Average(), 1),
                ActiveSubscription: activeSub,
                club.CreatedAt
            );

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating club {ClubId} for user {UserId}", clubId, userId);
            return Result.Failure<ClubResponse>(ClubErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Delete Club (soft delete)
    // ════════════════════════════════════════════════════════════════

    public async Task<Result> DeleteClubAsync(
        string userId, string clubId, CancellationToken ct = default)
    {
        try
        {
            var club = await _context.Clubs
                .FirstOrDefaultAsync(c => c.Id == clubId && !c.IsDeleted, ct);

            if (club is null)
                return Result.Failure(ClubErrors.ClubNotFound);

            if (club.OwnerId != userId)
                return Result.Failure(ClubErrors.Unauthorized);

            club.IsDeleted = true;
            club.IsActive = false;

            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting club {ClubId} for user {UserId}", clubId, userId);
            return Result.Failure(ClubErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Toggle Active/Inactive
    // ════════════════════════════════════════════════════════════════

    public async Task<Result> ToggleClubStatusAsync(
        string userId, string clubId, CancellationToken ct = default)
    {
        try
        {
            var club = await _context.Clubs
                .FirstOrDefaultAsync(c => c.Id == clubId && !c.IsDeleted, ct);

            if (club is null)
                return Result.Failure(ClubErrors.ClubNotFound);

            if (club.OwnerId != userId)
                return Result.Failure(ClubErrors.Unauthorized);

            club.IsActive = !club.IsActive;
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while toggling status for club {ClubId} by user {UserId}", clubId, userId);
            return Result.Failure(ClubErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  My Clubs (owner dashboard — includes inactive)
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<PaginatedList<ClubResponse>>> GetMyClubsAsync(
    string userId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var reviewStats = _context.Reviews
                .Where(r => !r.IsDeleted)
                .GroupBy(r => r.Court.ClubId)
                .Select(g => new
                {
                    ClubId = g.Key,
                    Count = (int?)g.Count(),
                    Average = (double?)g.Average(r => (double)r.Rating)
                });

            var clubsQuery = _context.Clubs
                .Where(c => c.OwnerId == userId && !c.IsDeleted)
                .ApplyFilters(filters,
                    searchPredicate: c => c.Name != null && c.Name.Contains(filters.SearchValue!),
                    allowedSortColumns: AllowedClubSortColumns);

            var query =
                from c in clubsQuery
                join rs in reviewStats on c.Id equals rs.ClubId into ratingsGroup
                from rs in ratingsGroup.DefaultIfEmpty()
                select new ClubResponse(
                    c.Id,
                    c.Name,
                    c.LogoUrl,
                    c.Governorate,
                    c.City,
                    c.Address,
                    c.PhoneNumber,
                    c.Email,
                    c.IsActive,
                    new UserSummary(
                        c.OwnerId,
                        c.Owner.FullName,
                        c.Owner.UserProfile == null ? null : c.Owner.UserProfile.ProfilePictureUrl),
                    IsOwner: true,
                    CanManageCourts: true,
                    CourtsCount: c.Courts.Count(x => !x.IsDeleted),
                    ReviewsCount: rs.Count ?? 0,
                    AverageRating: rs.Average ?? 0,
                    ActiveSubscription: c.Subscriptions
                        .Where(s => !s.IsDeleted &&
                                    s.StartDate <= DateTime.UtcNow &&
                                    s.EndDate >= DateTime.UtcNow)
                        .Select(s => new ClubSubscriptionSummary(
                            s.Id,
                            new SubscriptionPlanSummary(s.PlanId, s.Plan.Name, s.Plan.MonthlyPrice, s.Plan.MaxCourts),
                            s.StartDate,
                            s.EndDate,
                            true))
                        .FirstOrDefault(),
                    c.CreatedAt
                );

            var result = await query.AsNoTracking().ToPaginatedListAsync(filters, ct);
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving clubs owned by user {UserId}", userId);
            return Result.Failure<PaginatedList<ClubResponse>>(ClubErrors.Error);
        }
    }
}
```

## File: Abstractions/Consts/DefaultRoles.cs
```csharp
namespace Sportiva.Abstractions.Consts;

public static class DefaultRoles
{
    public partial class Admin
    {
        public const string Name = nameof(Admin);
        public const string Id = "0191a4b6-c4fc-752e-9d95-40b5e4e68054";
        public const string ConcurrencyStamp = "0191a4b6-c4fc-752e-9d95-40b631d1866d";
    }
    public partial class Owner
    {
        public const string Name = nameof(Owner);
        public const string Id = "647f9fdc-4677-473b-a656-4deb7000478c";
        public const string ConcurrencyStamp = "fb32d7a4-c53f-421d-bbd3-2b00dd57fa1d";
    }
    public partial class Member
    {
        public const string Name = nameof(Member);
        public const string Id = "0191a4b6-c4fc-752e-9d95-40b7a5cb88f0";
        public const string ConcurrencyStamp = "0191a4b6-c4fc-752e-9d95-40b85cf3fd22";
    }

}
```

## File: Abstractions/Consts/DefaultUsers.cs
```csharp
namespace Sportiva.Abstractions.Consts;

public static class DefaultUsers
{
    public partial class Admin
    {
        public const string Id = "0191a4b6-c4fc-752e-9d95-40b30fa7a9b6";
        public const string Email = "sayed732004444@gmail.com";
        public const string PasswordHash = "AQAAAAIAAYagAAAAEKRku5u6K325Irl1Utujiuil/WUhjTvShS9mJLXxO+2v/GKrMT1Ofhdp/0taFUO2bA==";
        public const string SecurityStamp = "55BF92C9EF0249CDA210D85D1A851BC9";
        public const string ConcurrencyStamp = "0191a4b6-c4fc-752e-9d95-40b42a925b8e";
    }
}
```

## File: Abstractions/Consts/Permissions.cs
```csharp
namespace Sportiva.Abstractions.Consts;

public static class Permissions
{
    public static string Type { get; } = "permissions";
    // Permitions
    //public const string GetUsers = "users:read";
    //public const string AddUsers = "users:add";
    //public const string UpdateUsers = "users:update";

    //public const string GetRoles = "roles:read";
    //public const string AddRoles = "roles:add";
    //public const string UpdateRoles = "roles:update";

    //public const string GetProfile = "profile:read";
    //public const string UpdateProfile = "profile:update";

    //public const string GetJobs = "jobs:read";
    //public const string AddJobs = "jobs:add";
    //public const string UpdateJobs = "jobs:update";
    //public const string DeleteJobs = "jobs:delete";
    //public const string GetJobApplicants = "jobApplicants:read";

    //public const string GetMembershipUpgradeRequests = "membershipUpgradeRequests:read";
    //public const string ApproveMembershipUpgradeRequests = "membershipUpgradeRequests:approve";
    //public const string RejectMembershipUpgradeRequests = "membershipUpgradeRequests:reject";

    public static IList<string?> GetAllPermissions() =>
        typeof(Permissions).GetFields().Select(x => x.GetValue(x) as string).ToList();
}
```

## File: Abstractions/Consts/RegexPatterns.cs
```csharp
namespace Sportiva.Abstractions.Consts;

public static class RegexPatterns
{
    public const string Password = "(?=(.*[0-9]))(?=.*[\\!@#$%^&*()\\\\[\\]{}\\-_+=~`|:;\"'<>,./?])(?=.*[a-z])(?=(.*[A-Z]))(?=(.*)).{8,}";
    public const string EgyptPhone = "^(\\+20|0020|0)?1[0125][0-9]{8}$";
}
```

## File: Abstractions/Error.cs
```csharp
namespace Sportiva.Abstractions;

public record Error(string Code, string Description, int? StatusCode)
{
    public static readonly Error None = new(string.Empty, string.Empty, null);
}
```

## File: Abstractions/PaginatedList.cs
```csharp
namespace Sportiva.Abstractions;

public sealed class PaginatedList<T>
{
    public List<T> Items { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    private PaginatedList(List<T> items, int pageNumber, int pageSize, int totalCount)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
    }

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {

        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageNumber, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);

        var totalCount = await source.CountAsync(cancellationToken);
        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<T>(items, pageNumber, pageSize, totalCount);

    }
}
```

## File: Abstractions/Result.cs
```csharp
namespace Sportiva.Abstractions;

public class Result
{
    public Result(bool isSuccess, Error error)
    {
        if ((isSuccess && error != Error.None) || (!isSuccess && error == Error.None))
            throw new InvalidOperationException();

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; } = default!;

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);

    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);
    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    public Result(TValue? value, bool isSuccess, Error error) : base(isSuccess, error)
    {
        _value = value;
    }

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Failure results cannot have value");
}
```

## File: Abstractions/ResultExtensions.cs
```csharp
namespace Sportiva.Abstractions;

public static class ResultExtensions
{
    public static ObjectResult ToProblem(this Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Cannot convert success result to a problem");

        var problem = Results.Problem(statusCode: result.Error.StatusCode);
        var problemDetails = problem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(problem) as ProblemDetails;

        problemDetails!.Extensions = new Dictionary<string, object?>
        {
            {
                "errors", new[]
                {
                    result.Error.Code,
                    result.Error.Description
                }
            }
        };

        return new ObjectResult(problemDetails);
    }
}
```

## File: Authentication/Filters/HasPermissionAttribute.cs
```csharp
namespace Sportiva.Authentication.Filters;

public class HasPermissionAttribute(string permission) : AuthorizeAttribute(permission)
{
}
```

## File: Authentication/Filters/PermissionAuthorizationHandler.cs
```csharp
namespace Sportiva.Authentication.Filters;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        //var user = context.User.Identity;

        //if(user is null || !user.IsAuthenticated)
        //    return;

        //var hasPermission = context.User.Claims.Any(x => x.Value == requirement.Permission && x.Type == Permissions.Type);

        //if(!hasPermission) 
        //    return;

        if (context.User.Identity is not { IsAuthenticated: true } ||
            !context.User.Claims.Any(x => x.Value == requirement.Permission && x.Type == Permissions.Type))
            return;

        context.Succeed(requirement);
        return;
    }
}
```

## File: Authentication/Filters/PermissionAuthorizationPolicyProvider.cs
```csharp
namespace Sportiva.Authentication.Filters;

public class PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
    : DefaultAuthorizationPolicyProvider(options)
{
    private readonly AuthorizationOptions _authorizationOptions = options.Value;

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var policy = await base.GetPolicyAsync(policyName);

        if (policy is not null)
            return policy;

        var permissionPolicy = new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(policyName))
            .Build();

        _authorizationOptions.AddPolicy(policyName, permissionPolicy);

        return permissionPolicy;
    }
}
```

## File: Authentication/Filters/PermissionRequirement.cs
```csharp
namespace Sportiva.Authentication.Filters;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}
```

## File: Authentication/IJwtProvider.cs
```csharp
namespace Sportiva.Authentication;

public interface IJwtProvider
{
    (string token, int expiresIn) GenerateToken(ApplicationUser user, IEnumerable<string> roles, IEnumerable<string> permissions);
    string? ValidateToken(string token, bool validateLifetime = true);
}
```

## File: Authentication/JwtOptions.cs
```csharp
namespace Sportiva.Authentication;

public class JwtOptions
{
    public static string SectionName = "Jwt";

    [Required]
    public string Key { get; init; } = string.Empty;

    [Required]
    public string Issuer { get; init; } = string.Empty;

    [Required]
    public string Audience { get; init; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int ExpiryMinutes { get; init; }
}
```

## File: CancellationExceptionFilter.cs
```csharp
using Microsoft.AspNetCore.Mvc.Filters;

// CancellationExceptionFilter.cs
public class CancellationExceptionFilter : IAsyncExceptionFilter
{
    public Task OnExceptionAsync(ExceptionContext context)
    {
        if (context.Exception is OperationCanceledException)
        {
            context.Result = new StatusCodeResult(499);
            context.ExceptionHandled = true;
        }
        return Task.CompletedTask;
    }
}
```

## File: Contracts/Authentication/AuthResponse.cs
```csharp
namespace Sportiva.Contracts.Authentication;

public record AuthResponse(
    string Id,
    string? Email,
    string FirstName,
    string LastName,
    string Token,
    int ExpiresIn,
    string RefreshToken,
    DateTime RefreshTokenExpiration
);
```

## File: Contracts/Authentication/ConfirmEmailRequest.cs
```csharp
namespace Sportiva.Contracts.Authentication;

public record ConfirmEmailRequest(
    string UserId,
    string Code
);
```

## File: Contracts/Authentication/ConfirmEmailRequestValidator.cs
```csharp
namespace Sportiva.Contracts.Authentication;

public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
{
    public ConfirmEmailRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Code)
            .NotEmpty();
    }
}
```

## File: Contracts/Authentication/ForgetPasswordRequest.cs
```csharp
namespace Sportiva.Contracts.Authentication;

public record ForgetPasswordRequest(
    string Email
);
```

## File: Contracts/Authentication/ForgetPasswordRequestValidator.cs
```csharp
namespace Sportiva.Contracts.Authentication;

public class ForgetPasswordRequestValidator : AbstractValidator<ForgetPasswordRequest>
{
    public ForgetPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
```

## File: Contracts/Authentication/LoginRequest.cs
```csharp
namespace Sportiva.Contracts.Authentication;

public record LoginRequest(
    string Email,
    string Password
);
```

## File: Contracts/Authentication/LoginRequestValidator.cs
```csharp
namespace Sportiva.Contracts.Authentication;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
```

## File: Contracts/Authentication/RefreshTokenRequest.cs
```csharp
namespace Sportiva.Contracts.Authentication;

public record RefreshTokenRequest(
    string Token,
    string RefreshToken
);
```

## File: Contracts/Authentication/RefreshTokenRequestValidator.cs
```csharp
namespace Sportiva.Contracts.Authentication;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
```

## File: Contracts/Authentication/RegisterRequest.cs
```csharp
namespace Sportiva.Contracts.Authentication;

public record RegisterRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName
);
```

## File: Contracts/Authentication/RegisterRequestValidator.cs
```csharp
namespace Sportiva.Contracts.Authentication;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .Matches(RegexPatterns.Password)
            .WithMessage("Password should be at least 8 digits and should contains Lowercase, NonAlphanumeric and Uppercase");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .Length(3, 100);
    }
}
```

## File: Contracts/Authentication/ResendConfirmationEmailRequest.cs
```csharp
namespace Sportiva.Contracts.Authentication;

public record ResendConfirmationEmailRequest(
    string Email
);
```

## File: Contracts/Authentication/ResendConfirmationEmailRequestValidator.cs
```csharp
namespace Sportiva.Contracts.Authentication;

public class ResendConfirmationEmailRequestValidator : AbstractValidator<ResendConfirmationEmailRequest>
{
    public ResendConfirmationEmailRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
```

## File: Contracts/Authentication/ResetPasswordRequest.cs
```csharp
namespace Sportiva.Contracts.Authentication;

public record ResetPasswordRequest(
    string Email,
    string Code,
    string NewPassword
);
```

## File: Contracts/Authentication/ResetPasswordRequestValidator.cs
```csharp
namespace Sportiva.Contracts.Authentication;

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Code)
           .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .Matches(RegexPatterns.Password)
            .WithMessage("Password should be at least 8 digits and should contains Lowercase, NonAlphanumeric and Uppercase");
    }
}
```

## File: Contracts/Bookings/BookingResponse.cs
```csharp
using Sportiva.Contracts.Shared.Enums;
using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Bookings;

public record BookingResponse(
    string           BookingId,
    string           BookingNumber,
    BookingStatusDto Status,
    decimal          Price,

    CourtSummary    Court,
    TimeSlotSummary TimeSlot,

    UserSummary BookedBy,

    bool IsMine,
    bool CanCancel,
    bool CanReview,

    ReviewSummary? ExistingReview,

    DateTime CreatedAt
);
```

## File: Contracts/Bookings/CreateBookingRequest.cs
```csharp
namespace Sportiva.Contracts.Bookings;

public record CreateBookingRequest(
    string CourtId,
    string TimeSlotId
);
```

## File: Contracts/Clubs/ClubResponse.cs
```csharp
using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Clubs;

public record ClubResponse(
    string  ClubId,
    string? Name,
    string? LogoUrl,
    string? Governorate,
    string? City,
    string? Address,
    string? PhoneNumber,
    string? Email,
    bool    IsActive,

    UserSummary Owner,

    bool IsOwner,
    bool CanManageCourts,

    int    CourtsCount,
    int    ReviewsCount,
    double AverageRating,

    ClubSubscriptionSummary? ActiveSubscription,

    DateTime CreatedAt
);
```

## File: Contracts/Clubs/ClubSubscriptionSummary.cs
```csharp
using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Clubs;

public record ClubSubscriptionSummary(
    string                  SubscriptionId,
    SubscriptionPlanSummary Plan,
    DateTime                StartDate,
    DateTime                EndDate,
    bool                    IsActive
);
```

## File: Contracts/Common/RequestFilters.cs
```csharp
namespace Sportiva.Contracts.Common;

public enum SortDirection
{
    Asc,
    Desc
}

public record RequestFilters
{
    private const int MaxPageSize = 50;
    private int _pageSize = 10;
    public int PageNumber { get; init; } = 1;

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    public string? SearchValue { get; init; }
    public string? SortColumn { get; init; }
    public SortDirection SortDirection { get; init; } = SortDirection.Asc;
}
```

## File: Contracts/Courts/CourtResponse.cs
```csharp
using Sportiva.Contracts.Shared.Enums;
using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Courts;

public record CourtResponse(
    string       CourtId,
    string?      Name,
    string?      Description,
    string?      ImageUrl,
    SportTypeDto SportType,
    int          MaxCapacity,
    decimal      PricePerHour,
    bool         IsActive,

    ClubSummary Club,

    bool CanBook,
    bool CanManage,

    int    ReviewsCount,
    double AverageRating,

    DateTime CreatedAt
);
```

## File: Contracts/Matches/CreateFriendlyMatchRequest.cs
```csharp
using Sportiva.Contracts.Shared.Enums;

namespace Sportiva.Contracts.Matches;

public record CreateFriendlyMatchRequest(
    string       CourtId,
    DateOnly     Date,
    TimeOnly     StartTime,
    TimeOnly     EndTime,
    SportTypeDto SportType,
    int          RequiredPlayers,
    string?      Note
);
```

## File: Contracts/Matches/FriendlyMatchResponse.cs
```csharp
using Sportiva.Contracts.Shared.Enums;
using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Matches;

public record FriendlyMatchResponse(
    string         MatchId,
    DateOnly       Date,
    TimeOnly       StartTime,
    TimeOnly       EndTime,
    SportTypeDto   SportType,
    int            RequiredPlayers,
    int            AcceptedPlayersCount,
    int            SlotsRemaining,
    MatchStatusDto Status,
    string?        Note,

    CourtSummary Court,
    UserSummary  Organizer,

    bool IsOwner,
    bool IParticipating,
    bool IApplied,
    bool CanJoin,

    IReadOnlyList<ParticipantSummary> ParticipantsPreview,   // capped at 5

    DateTime CreatedAt
);
```

## File: Contracts/Matches/JoinMatchRequest.cs
```csharp
namespace Sportiva.Contracts.Matches;

public record JoinMatchRequest(
    string FriendlyMatchId
);
```

## File: Contracts/Matches/MatchJoinRequestResponse.cs
```csharp
using Sportiva.Contracts.Shared.Enums;
using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Matches;

public record MatchJoinRequestResponse(
    string               RequestId,
    JoinRequestStatusDto Status,
    UserSummary          Player,
    FriendlyMatchSummary Match,
    bool                 IsMine,
    DateTime             CreatedAt
);
```

## File: Contracts/Matches/ReviewJoinRequestRequest.cs
```csharp
using Sportiva.Contracts.Shared.Enums;

namespace Sportiva.Contracts.Matches;

public record ReviewJoinRequestRequest(
    JoinRequestStatusDto NewStatus
);
```

## File: Contracts/Memberships/CreateMembershipUpgradeRequest.cs
```csharp
namespace Sportiva.Contracts.Memberships;

public record CreateMembershipUpgradeRequest(
    string  FullName,
    string  Phone,
    bool    IsClubOwner,
    string? ClubName,
    string? Address,
    string? LocationUrl,
    string? Note
);
```

## File: Contracts/Memberships/MembershipUpgradeResponse.cs
```csharp
using Sportiva.Contracts.Shared.Enums;
using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Memberships;

public record MembershipUpgradeResponse(
    string           RequestId,
    RequestStatusDto Status,
    string           FullName,
    string           Phone,
    bool             IsClubOwner,
    string?          ClubName,
    string?          Address,
    string?          LocationUrl,
    string?          Note,
    UserSummary      RequestedBy,
    bool             IsMine,
    DateTime         CreatedAt,
    DateTime?        ReviewedAt
);
```

## File: Contracts/Memberships/ReviewMembershipUpgradeRequest.cs
```csharp
using Sportiva.Contracts.Shared.Enums;

namespace Sportiva.Contracts.Memberships;

public record ReviewMembershipUpgradeRequest(
    RequestStatusDto NewStatus
);
```

## File: Contracts/Messaging/ConversationSummary.cs
```csharp
using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Messaging;

public record ConversationSummary(
    UserSummary OtherParty,
    string      LastMessagePreview,
    bool        LastMessageIsMine,
    int         UnreadCount,
    DateTime    LastMessageAt
);
```

## File: Contracts/Messaging/MessageResponse.cs
```csharp
using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Messaging;

public record MessageResponse(
    string      MessageId,
    string      Content,
    UserSummary Sender,
    bool        IsMine,
    bool        IsRead,
    DateTime    SentAt
);
```

## File: Contracts/Messaging/SendMessageRequest.cs
```csharp
namespace Sportiva.Contracts.Messaging;

public record SendMessageRequest(
    string ReceiverId,
    string Content
);
```

## File: Contracts/Notifications/BulkUpdateNotificationPreferencesRequest.cs
```csharp
namespace Sportiva.Contracts.Notifications;

public record BulkUpdateNotificationPreferencesRequest(
    IReadOnlyList<NotificationPreferenceItem> Preferences
);
```

## File: Contracts/Notifications/NotificationCountersResponse.cs
```csharp
namespace Sportiva.Contracts.Notifications;

public record NotificationCountersResponse(
    int UnreadCount,
    int TotalCount
);
```

## File: Contracts/Notifications/NotificationListResponse.cs
```csharp
namespace Sportiva.Contracts.Notifications;

public record NotificationListResponse(
    IReadOnlyList<NotificationResponse> Items,
    int  TotalCount,
    int  UnreadCount,
    int  PageNumber,
    int  PageSize,
    bool HasMore
);
```

## File: Contracts/Notifications/NotificationPreferenceItem.cs
```csharp
using Sportiva.Contracts.Shared.Enums;

namespace Sportiva.Contracts.Notifications;

public record NotificationPreferenceItem(
    NotificationTypeDto Type,
    bool                InAppEnabled,
    bool                EmailEnabled
);
```

## File: Contracts/Notifications/NotificationPreferenceResponse.cs
```csharp
using Sportiva.Contracts.Shared.Enums;

namespace Sportiva.Contracts.Notifications;

public record NotificationPreferenceResponse(
    NotificationTypeDto Type,
    bool                InAppEnabled,
    bool                EmailEnabled
);
```

## File: Contracts/Notifications/NotificationPreferencesListResponse.cs
```csharp
namespace Sportiva.Contracts.Notifications;

public record NotificationPreferencesListResponse(
    IReadOnlyList<NotificationPreferenceResponse> Preferences
);
```

## File: Contracts/Notifications/NotificationResponse.cs
```csharp
using Sportiva.Contracts.Shared.Enums;
using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Notifications;

public record NotificationResponse(
    string                   NotificationId,
    NotificationTypeDto      Type,
    NotificationPriorityDto? Priority,    // optional — use for styling (e.g. SecurityAlert = High)
    string                   Title,
    string                   Body,
    UserSummary?             Actor,
    string?                  EntityType,
    string?                  EntityId,
    bool                     IsRead,
    DateTime?                ReadAt,
    DateTime                 CreatedAt
);
```

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

## File: Contracts/Reviews/CreateReviewRequest.cs
```csharp
namespace Sportiva.Contracts.Reviews;

public record CreateReviewRequest(
    string  BookingId,
    int     Rating,
    string? Comment
);
```

## File: Contracts/Reviews/ReviewResponse.cs
```csharp
using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Reviews;

public record ReviewResponse(
    string       ReviewId,
    int          Rating,
    string?      Comment,
    CourtSummary Court,
    UserSummary  Author,
    bool         IsOwner,
    DateTime     CreatedAt
);
```

## File: Contracts/Shared/Enums/BookingStatusDto.cs
```csharp
namespace Sportiva.Contracts.Shared.Enums;

public enum BookingStatusDto
{
    Pending,
    Confirmed,
    Cancelled,
    Completed
}
```

## File: Contracts/Shared/Enums/JoinRequestStatusDto.cs
```csharp
namespace Sportiva.Contracts.Shared.Enums;

public enum JoinRequestStatusDto
{
    Pending,
    Accepted,
    Rejected
}
```

## File: Contracts/Shared/Enums/MatchStatusDto.cs
```csharp
namespace Sportiva.Contracts.Shared.Enums;

public enum MatchStatusDto
{
    Open,
    Full,
    InProgress,
    Completed,
    Cancelled
}
```

## File: Contracts/Shared/Enums/NotificationPriorityDto.cs
```csharp
namespace Sportiva.Contracts.Shared.Enums;

public enum NotificationPriorityDto
{
    Low,
    Normal,
    High
}
```

## File: Contracts/Shared/Enums/NotificationTypeDto.cs
```csharp
namespace Sportiva.Contracts.Shared.Enums;

public enum NotificationTypeDto
{
    NewFollower,
    PostLiked,
    PostCommented,
    CommentReplied,
    CommentReacted,
    BookingConfirmed,
    BookingCancelled,
    BookingReminder,
    MatchJoinRequestReceived,
    MatchJoinRequestAccepted,
    MatchJoinRequestRejected,
    MatchFull,
    NewMessage,
    SecurityAlert,
    GeneralInfo
}
```

## File: Contracts/Shared/Enums/PaymentStatusDto.cs
```csharp
namespace Sportiva.Contracts.Shared.Enums;

public enum PaymentStatusDto
{
    Pending,
    Paid,
    Failed,
    Refunded
}
```

## File: Contracts/Shared/Enums/RequestStatusDto.cs
```csharp
namespace Sportiva.Contracts.Shared.Enums;

public enum RequestStatusDto
{
    Pending,
    Approved,
    Rejected
}
```

## File: Contracts/Shared/Enums/SportTypeDto.cs
```csharp
namespace Sportiva.Contracts.Shared.Enums;

public enum SportTypeDto
{
    Football,
    Basketball,
    Tennis,
    Padel,
    Volleyball,
    Other
}
```

## File: Contracts/Shared/Enums/TournamentStatusDto.cs
```csharp
namespace Sportiva.Contracts.Shared.Enums;

public enum TournamentStatusDto
{
    Upcoming,
    Ongoing,
    Completed
}
```

## File: Contracts/Shared/Summaries/ClubSummary.cs
```csharp
namespace Sportiva.Contracts.Shared.Summaries;

public record ClubSummary(
    string  ClubId,
    string? Name,
    string? LogoUrl,
    string? City,
    string? Governorate
);
```

## File: Contracts/Shared/Summaries/CourtSummary.cs
```csharp
using Sportiva.Contracts.Shared.Enums;

namespace Sportiva.Contracts.Shared.Summaries;

public record CourtSummary(
    string       CourtId,
    string?      Name,
    string?      ImageUrl,
    SportTypeDto SportType,
    decimal      PricePerHour,
    ClubSummary  Club
);
```

## File: Contracts/Shared/Summaries/FriendlyMatchSummary.cs
```csharp
using Sportiva.Contracts.Shared.Enums;

namespace Sportiva.Contracts.Shared.Summaries;

public record FriendlyMatchSummary(
    string       MatchId,
    DateOnly     Date,
    TimeOnly     StartTime,
    TimeOnly     EndTime,
    SportTypeDto SportType,
    CourtSummary Court
);
```

## File: Contracts/Shared/Summaries/ParticipantSummary.cs
```csharp
namespace Sportiva.Contracts.Shared.Summaries;

public record ParticipantSummary(
    string   UserId,
    string   FullName,
    string?  ProfilePictureUrl,
    DateTime JoinedAt
);
```

## File: Contracts/Shared/Summaries/ReviewSummary.cs
```csharp
namespace Sportiva.Contracts.Shared.Summaries;

public record ReviewSummary(
    string   ReviewId,
    int      Rating,
    string?  Comment,
    DateTime CreatedAt
);
```

## File: Contracts/Shared/Summaries/SubscriptionPlanSummary.cs
```csharp
namespace Sportiva.Contracts.Shared.Summaries;

public record SubscriptionPlanSummary(
    string  PlanId,
    string  Name,
    decimal MonthlyPrice,
    int     MaxCourts
);
```

## File: Contracts/Shared/Summaries/TimeSlotSummary.cs
```csharp
namespace Sportiva.Contracts.Shared.Summaries;

public record TimeSlotSummary(
    string   TimeSlotId,
    DateOnly Day,
    TimeOnly StartTime,
    TimeOnly EndTime,
    bool     IsBooked
);
```

## File: Contracts/Shared/Summaries/UserCardSummary.cs
```csharp
namespace Sportiva.Contracts.Shared.Summaries;

public record UserCardSummary(
    string UserId,
    string FullName,
    string? ProfilePictureUrl,
    string? Bio,
    string? City,
    bool IsFollowing,
    bool IsMe,
    DateTime? FollowedAt        // null when !IsFollowing
);
```

## File: Contracts/Shared/Summaries/UserSummary.cs
```csharp
namespace Sportiva.Contracts.Shared.Summaries;

public record UserSummary(
    string  UserId,
    string  FullName,
    string? ProfilePictureUrl
);
```

## File: Contracts/Subscriptions/ClubSubscriptionResponse.cs
```csharp
using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Subscriptions;

public record ClubSubscriptionResponse(
    string                      SubscriptionId,
    ClubSummary                 Club,
    SubscriptionPlanSummary     Plan,
    DateTime                    StartDate,
    DateTime                    EndDate,
    bool                        IsActive,
    int                         PaymentsCount,
    SubscriptionPaymentSummary? LastPayment
);
```

## File: Contracts/Subscriptions/CreateClubSubscriptionRequest.cs
```csharp
namespace Sportiva.Contracts.Subscriptions;

public record CreateClubSubscriptionRequest(
    string   ClubId,
    string   PlanId,
    DateTime StartDate,
    DateTime EndDate
);
```

## File: Contracts/Subscriptions/SubscriptionPaymentSummary.cs
```csharp
using Sportiva.Contracts.Shared.Enums;

namespace Sportiva.Contracts.Subscriptions;

public record SubscriptionPaymentSummary(
    string           PaymentId,
    decimal          Amount,
    PaymentStatusDto Status,
    string?          TransactionId,
    DateTime?        PaidAt
);
```

## File: Contracts/Subscriptions/SubscriptionPlanResponse.cs
```csharp
namespace Sportiva.Contracts.Subscriptions;

public record SubscriptionPlanResponse(
    string    PlanId,
    string    Name,
    string?   Description,
    decimal   MonthlyPrice,
    int       MaxCourts,
    bool      IsActive,
    DateTime? ExpiresAt,
    DateTime  CreatedAt
);
```

## File: Contracts/TimeSlots/CreateTimeSlotRequest.cs
```csharp
namespace Sportiva.Contracts.TimeSlots;

public record CreateTimeSlotRequest(
    string   CourtId,
    DateOnly Day,
    TimeOnly StartTime,
    TimeOnly EndTime
);
```

## File: Contracts/TimeSlots/TimeSlotResponse.cs
```csharp
using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.TimeSlots;

public record TimeSlotResponse(
    string       TimeSlotId,
    CourtSummary Court,
    DateOnly     Day,
    TimeOnly     StartTime,
    TimeOnly     EndTime,
    bool         IsBooked,
    DateTime     CreatedAt
);
```

## File: Contracts/Tournaments/CreateTournamentMatchRequest.cs
```csharp
namespace Sportiva.Contracts.Tournaments;

public record CreateTournamentMatchRequest(
    string   TournamentId,
    string   Player1Id,
    string   Player2Id,
    int?     Round,
    int?     MatchNumber,
    DateOnly MatchDate,
    TimeOnly StartTime
);
```

## File: Contracts/Tournaments/CreateTournamentRequest.cs
```csharp
using Sportiva.Contracts.Shared.Enums;

namespace Sportiva.Contracts.Tournaments;

public record CreateTournamentRequest(
    string       Name,
    string?      Description,
    SportTypeDto SportType,
    DateOnly     StartDate,
    DateOnly     EndDate,
    int          MaxParticipants
);
```

## File: Contracts/Tournaments/SetTournamentMatchWinnerRequest.cs
```csharp
namespace Sportiva.Contracts.Tournaments;

public record SetTournamentMatchWinnerRequest(
    string WinnerId
);
```

## File: Contracts/Tournaments/TournamentMatchResponse.cs
```csharp
using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Tournaments;

public record TournamentMatchResponse(
    string       MatchId,
    string       TournamentId,
    int?         Round,
    int?         MatchNumber,
    UserSummary  Player1,
    UserSummary  Player2,
    UserSummary? Winner,
    bool         IsDecided,
    DateOnly     MatchDate,
    TimeOnly     StartTime
);
```

## File: Contracts/Tournaments/TournamentResponse.cs
```csharp
using Sportiva.Contracts.Shared.Enums;
using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Tournaments;

public record TournamentResponse(
    string              TournamentId,
    string              Name,
    string?             Description,
    SportTypeDto        SportType,
    TournamentStatusDto Status,
    DateOnly            StartDate,
    DateOnly            EndDate,
    int                 MaxParticipants,

    UserSummary Organizer,

    bool IsOwner,
    bool IParticipating,
    bool CanJoin,

    int ParticipantsCount,
    int MatchesCount,
    int CompletedMatchesCount,

    DateTime CreatedAt
);
```

## File: Contracts/Users/ToggleFollowResponse.cs
```csharp
namespace Sportiva.Contracts.Users;

public record ToggleFollowResponse(
 string TargetUserId,
 bool IsNowFollowing,
 int FollowersCount
);
```

## File: Contracts/Users/UpdateProfileCoverRequest.cs
```csharp
// UpdateProfileCoverRequest.cs
namespace Sportiva.Contracts.Users;

public record UpdateProfileCoverRequest(
    IFormFile CoverImage
);
```

## File: Contracts/Users/UpdateProfileInfoRequest.cs
```csharp
// UpdateProfileInfoRequest.cs
using Sportiva.Contracts.Shared.Enums;

namespace Sportiva.Contracts.Users;

public record UpdateProfileInfoRequest(
    string? FirstName,
    string? LastName,
    string? Bio,
    string? City,
    string? Country,
    SportTypeDto? PreferredSport,
    string? PreferredCity
);
```

## File: Contracts/Users/UpdateProfilePhotoRequest.cs
```csharp
// UpdateProfilePhotoRequest.cs
namespace Sportiva.Contracts.Users;

public record UpdateProfilePhotoRequest(
    IFormFile ProfilePicture
);
```

## File: Contracts/Users/UserProfileResponse.cs
```csharp
using Sportiva.Contracts.Shared.Enums;

namespace Sportiva.Contracts.Users;

public record UserProfileResponse(
    // — Identity
    string UserId,
    string FirstName,
    string LastName,
    string FullName,
    string Email,

    // — Profile
    string?       Bio,
    string?       City,
    string?       Country,
    string?       ProfilePictureUrl,
    string?       CoverImageUrl,
    SportTypeDto? PreferredSport,
    string?       PreferredCity,

    // — Current-user context
    bool IsMe,
    bool IsFollowing,
    bool CanSendMessage,

    // — Counters
    int FollowersCount,
    int FollowingCount,
    int PostsCount,

    // — Metadata
    DateTime CreatedAt
);
```

## File: Controllers/AuthController.cs
```csharp
using Microsoft.AspNetCore.Authentication.Google;
using Sportiva.Services;
namespace Sportiva.Controllers;

[Route("auth")]
[ApiController]
[Produces("application/json")]
public class AuthController(
    IAuthService authService,
    ILogger<AuthController> logger,
    IConfiguration configuration,
    SignInManager<ApplicationUser> signInManager) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly ILogger<AuthController> _logger = logger;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly string _frontendOrigin =
        configuration["AppSettings:FrontendOrigin"] ?? "https://front-end-project-bay-seven.vercel.app";

    [HttpGet("google-login")]
    public IActionResult GoogleLogin()
    {
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(
            GoogleDefaults.AuthenticationScheme,
            Url.Action(nameof(GoogleResponse))
        );
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("google-response")]
    public async Task<IActionResult> GoogleResponse()
    {
        var result = await _authService.HandleGoogleLoginAsync();
        return HandleOAuthCallback(result);
    }

    [HttpGet("github-login")]
    public IActionResult GitHubLogin()
    {
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(
            "GitHub",
            Url.Action(nameof(GitHubResponse))
        );
        return Challenge(properties, "GitHub");
    }

    [HttpGet("github-response")]
    public async Task<IActionResult> GitHubResponse()
    {
        var result = await _authService.HandleGitHubLoginAsync();
        return HandleOAuthCallback(result);
    }

    private IActionResult HandleOAuthCallback(Result<AuthResponse> result)
    {
        if (!result.IsSuccess)
            return Redirect($"{_frontendOrigin}/login?error=authentication_failed");

        var auth = result.Value;

        var redirectUrl = $"{_frontendOrigin}/oauth/callback" +
            $"?token={Uri.EscapeDataString(auth.Token)}" +
            $"&refreshToken={Uri.EscapeDataString(auth.RefreshToken)}" +
            $"&expiresIn={auth.ExpiresIn}" +
            $"&refreshTokenExpiration={Uri.EscapeDataString(auth.RefreshTokenExpiration.ToString())}" +
            $"&userId={Uri.EscapeDataString(auth.Id)}" +
            $"&email={Uri.EscapeDataString(auth.Email ?? "")}" +
            $"&firstName={Uri.EscapeDataString(auth.FirstName)}" +
            $"&lastName={Uri.EscapeDataString(auth.LastName)}";

        return Redirect(redirectUrl);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Logging with email: {email}", request.Email);
            var authResult = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);
            return authResult.IsSuccess ? Ok(authResult.Value) : authResult.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499);
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var authResult = await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
            return authResult.IsSuccess ? Ok(authResult.Value) : authResult.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499);
        }
    }

    [HttpPost("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
            return result.IsSuccess ? Ok() : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499);
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.RegisterAsync(request, cancellationToken);
            return result.IsSuccess ? Ok() : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499);
        }
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.ConfirmEmailAsync(request);
            return result.IsSuccess ? Ok() : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499);
        }
    }

    [HttpPost("resend-confirmation-email")]
    public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationEmailRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.ResendConfirmationEmailAsync(request);
            return result.IsSuccess ? Ok() : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499);
        }
    }

    [HttpPost("forget-password")]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest request)
    {
        try
        {
            var result = await _authService.SendResetPasswordCodeAsync(request.Email);
            return result.IsSuccess ? Ok() : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499);
        }
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        try
        {
            var result = await _authService.ResetPasswordAsync(request);
            return result.IsSuccess ? Ok() : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499);
        }
    }
}
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

## File: Controllers/ProfilesController.cs
```csharp
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
}
```

## File: Entities/ApplicationRole.cs
```csharp
namespace Sportiva.Entities;

public class ApplicationRole : IdentityRole
{
    public ApplicationRole()
    {
        Id = Guid.CreateVersion7().ToString();
    }

    public bool IsDefault { get; set; }
    public bool IsDeleted { get; set; }
}
```

## File: Entities/ApplicationUser.cs
```csharp
namespace Sportiva.Entities;

public sealed class ApplicationUser : IdentityUser
{
    public ApplicationUser()
    {
        Id = Guid.CreateVersion7().ToString();
        SecurityStamp = Guid.CreateVersion7().ToString();
    }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDisabled { get; set; }
    public UserProfile UserProfile { get; set; } = default!;
    public List<RefreshToken> RefreshTokens { get; set; } = [];
    public ICollection<Club> OwnedClubs { get; set; } = [];
    public ICollection<Booking> Bookings { get; set; } = [];
    public ICollection<Post> Posts { get; set; } = [];
    public ICollection<PostLike> PostLikes { get; set; } = [];
    public ICollection<MembershipUpgrade> MembershipUpgradeRequests { get; set; } = [];
    public ICollection<FriendlyMatch> OrganizedMatches { get; set; } = [];
    public ICollection<MatchJoinRequest> MatchJoinRequests { get; set; } = [];
    public ICollection<CommentReply> CommentReplies { get; set; } = [];
    public ICollection<CommentReaction> CommentReactions { get; set; } = [];
    public ICollection<ReplyReaction> ReplyReactions { get; set; } = [];
    // الناس اللي أنا بـ follow هم
    public ICollection<UserFollow> Following { get; set; } = [];

    // الناس اللي بيـ follow أنا
    public ICollection<UserFollow> Followers { get; set; } = [];
}
```

## File: Entities/Booking.cs
```csharp
namespace Sportiva.Entities;

public class Booking
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    public string CourtId { get; set; } = string.Empty;
    public Court Court { get; set; } = default!;

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = default!;

    public string TimeSlotId { get; set; } = string.Empty;
    public TimeSlot TimeSlot { get; set; } = default!;

    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    public decimal Price { get; set; }  // سعر الملعب وقت الحجز للعرض فقط
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public bool IsDeleted { get; set; } = false;
}
```

## File: Entities/Club.cs
```csharp
namespace Sportiva.Entities;

public class Club
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string? Name { get; set; }
    public string? LogoUrl { get; set; }
    public string? Governorate { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // ✅ الإضافة المطلوبة

    public string OwnerId { get; set; } = string.Empty;
    public ApplicationUser Owner { get; set; } = default!;

    public ICollection<Court> Courts { get; set; } = [];
    public ICollection<ClubSubscription> Subscriptions { get; set; } = [];
}
```

## File: Entities/ClubSubscription.cs
```csharp
namespace Sportiva.Entities;

public class ClubSubscription
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    public string ClubId { get; set; } = string.Empty;
    public Club Club { get; set; } = default!;

    public string PlanId { get; set; } = string.Empty;
    public SubscriptionPlan Plan { get; set; } = default!;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsDeleted { get; set; } = false;
    public bool IsActive => !IsDeleted && DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate;

    public ICollection<SubscriptionPayment> Payments { get; set; } = [];
}
```

## File: Entities/CommentReaction.cs
```csharp
namespace Sportiva.Entities;

public class CommentReaction
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    public string CommentId { get; set; } = string.Empty;
    public PostComment Comment { get; set; } = default!;

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = default!;

    public DateTime LikedAt { get; set; } = DateTime.UtcNow;
}
```

## File: Entities/CommentReply.cs
```csharp
namespace Sportiva.Entities;

public class CommentReply
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    public string CommentId { get; set; } = string.Empty;
    public PostComment Comment { get; set; } = default!;

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = default!;

    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;

    public ICollection<ReplyReaction> Reactions { get; set; } = [];
}
```

## File: Entities/Court.cs
```csharp
namespace Sportiva.Entities;

public class Court
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    public string ClubId { get; set; } = string.Empty;
    public Club Club { get; set; } = default!;                      // ✅ تم إضافة = default!

    public string? Name { get; set; }
    public decimal PricePerHour { get; set; }
    public string? Description { get; set; }
    public SportType SportType { get; set; } = SportType.Football;
    public int MaxCapacity { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; } = false;
    public string? ImageUrl { get; set; }

    public ICollection<TimeSlot> TimeSlots { get; set; } = [];     // ✅ تم تهيئة الـ collection
}
```

## File: Entities/FriendlyMatch.cs
```csharp
namespace Sportiva.Entities;

public class FriendlyMatch
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string OrganizerId { get; set; } = string.Empty;
    public ApplicationUser Organizer { get; set; } = default!;
    public string CourtId { get; set; } = string.Empty;
    public Court Court { get; set; } = default!;

    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public SportType SportType { get; set; } = SportType.Football;
    public int RequiredPlayers { get; set; }
    public MatchStatus Status { get; set; } = MatchStatus.Open;
    public string? Note { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<MatchJoinRequest> JoinRequests { get; set; } = [];
}
```

## File: Entities/MatchJoinRequest.cs
```csharp
namespace Sportiva.Entities;

public class MatchJoinRequest
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string FriendlyMatchId { get; set; } = string.Empty;
    public FriendlyMatch FriendlyMatch { get; set; } = default!;
    public string PlayerId { get; set; } = string.Empty;
    public ApplicationUser Player { get; set; } = default!;
    public JoinRequestStatus Status { get; set; } = JoinRequestStatus.Pending;
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
}
```

## File: Entities/MembershipUpgrade.cs
```csharp
namespace Sportiva.Entities;

public class MembershipUpgrade
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = default!;
    public RequestStatus Status { get; set; } = RequestStatus.Pending;
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public bool IsClubOwner { get; set; } = false;
    public string? ClubName { get; set; }
    public string? Address { get; set; }
    public string? LocationUrl { get; set; }
    public string? Note { get; set; }
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
}
```

## File: Entities/Message.cs
```csharp
namespace Sportiva.Entities;

public sealed class Message
{
    public int Id { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public ApplicationUser Sender { get; set; } = null!;
    public string ReceiverId { get; set; } = string.Empty;
    public ApplicationUser Receiver { get; set; } = null!;
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; }
}
```

## File: Entities/Notification.cs
```csharp
namespace Sportiva.Entities;

public enum NotificationType
{
    // Social
    NewFollower,
    PostLiked,
    PostCommented,
    CommentReplied,
    CommentReacted,

    // Booking
    BookingConfirmed,
    BookingCancelled,
    BookingReminder,

    // Friendly Match
    MatchJoinRequestReceived,
    MatchJoinRequestAccepted,
    MatchJoinRequestRejected,
    MatchFull,

    // Chat
    NewMessage,

    // System
    SecurityAlert,
    GeneralInfo
}

public enum NotificationPriority
{
    Low,
    Normal,
    High
}

public class Notification
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    public string RecipientId { get; set; } = string.Empty;
    public ApplicationUser Recipient { get; set; } = default!;

    public string? ActorId { get; set; }
    public ApplicationUser? Actor { get; set; }

    public NotificationType Type { get; set; }
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public string? EntityType { get; set; }
    public string? EntityId { get; set; }

    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
    public bool EmailSent { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

## File: Entities/NotificationPreference.cs
```csharp
namespace Sportiva.Entities;

public class NotificationPreference
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = default!;

    public NotificationType Type { get; set; }

    public bool InAppEnabled { get; set; } = true;
    public bool EmailEnabled { get; set; } = false;
}
```

## File: Entities/Post.cs
```csharp
namespace Sportiva.Entities;

public class Post
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = default!;
    public string Content { get; set; } = string.Empty;
    public string? FileUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public ICollection<PostLike> Likes { get; set; } = [];
    public ICollection<PostComment> Comments { get; set; } = [];
}
```

## File: Entities/PostComment.cs
```csharp
namespace Sportiva.Entities;

public class PostComment
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    public string PostId { get; set; } = string.Empty;
    public Post Post { get; set; } = default!;

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = default!;

    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;

    public ICollection<CommentReaction> Reactions { get; set; } = [];
    public ICollection<CommentReply> Replies { get; set; } = [];
}
```

## File: Entities/PostLike.cs
```csharp
namespace Sportiva.Entities;

public class PostLike
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string PostId { get; set; } = string.Empty;
    public Post Post { get; set; } = default!;
    public string UserId { get; set; } = string.Empty;
    // public bool IsDeleted { get; set; } = false;
    public ApplicationUser User { get; set; } = default!;
    public DateTime LikedAt { get; set; } = DateTime.UtcNow;
}
```

## File: Entities/RefreshToken.cs
```csharp
namespace Sportiva.Entities;

public class RefreshToken
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresOn { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedOn { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
    public bool IsActive => RevokedOn is null && !IsExpired;
}
```

## File: Entities/ReplyReaction.cs
```csharp
namespace Sportiva.Entities;

public class ReplyReaction
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    public string ReplyId { get; set; } = string.Empty;
    public CommentReply Reply { get; set; } = default!;

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = default!;

    public DateTime LikedAt { get; set; } = DateTime.UtcNow;
}
```

## File: Entities/Review.cs
```csharp
namespace Sportiva.Entities;

public class Review
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string CourtId { get; set; } = string.Empty;
    public Court Court { get; set; } = default!;
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = default!;
    public string BookingId { get; set; } = string.Empty;
    public Booking Booking { get; set; } = default!;
    public int Rating { get; set; }               // من 1 لـ 5
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
}
```

## File: Entities/SubscriptionPayment.cs
```csharp
namespace Sportiva.Entities;

public class SubscriptionPayment
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    public string ClubSubscriptionId { get; set; } = string.Empty;
    public ClubSubscription ClubSubscription { get; set; } = default!;

    public decimal Amount { get; set; }
    public DateTime? PaidAt { get; set; }

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? TransactionId { get; set; }
}
```

## File: Entities/SubscriptionPlan.cs
```csharp
namespace Sportiva.Entities;

public class SubscriptionPlan
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal MonthlyPrice { get; set; }
    public int MaxCourts { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // ✅ الإضافة

    public ICollection<ClubSubscription> ClubSubscriptions { get; set; } = [];
}
```

## File: Entities/TimeSlot.cs
```csharp
namespace Sportiva.Entities;

public class TimeSlot
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string CourtId { get; set; } = string.Empty;
    public Court Court { get; set; } = default!;
    public DateOnly Day { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;

    public ICollection<Booking> Bookings { get; set; } = [];
    public bool IsBooked => Bookings.Any(b =>
        !b.IsDeleted &&
        (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending));
}
```

## File: Entities/Tournament.cs
```csharp
namespace Sportiva.Entities;

public class Tournament
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string OrganizerId { get; set; } = string.Empty;
    public ApplicationUser Organizer { get; set; } = default!;
    public SportType SportType { get; set; } = SportType.Football;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int MaxParticipants { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<TournamentParticipant> Participants { get; set; } = [];
    public ICollection<TournamentMatch> Matches { get; set; } = [];
}
```

## File: Entities/TournamentMatch.cs
```csharp
namespace Sportiva.Entities;

public class TournamentMatch
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string TournamentId { get; set; } = string.Empty;
    public Tournament Tournament { get; set; } = default!;
    public string Player1Id { get; set; } = string.Empty;
    public ApplicationUser Player1 { get; set; } = default!;
    public string Player2Id { get; set; } = string.Empty;
    public ApplicationUser Player2 { get; set; } = default!;
    public string? WinnerId { get; set; }
    public ApplicationUser? Winner { get; set; }
    public DateOnly MatchDate { get; set; }
    public TimeOnly StartTime { get; set; }
}
```

## File: Entities/TournamentParticipant.cs
```csharp
namespace Sportiva.Entities;

public class TournamentParticipant
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string TournamentId { get; set; } = string.Empty;
    public Tournament Tournament { get; set; } = default!;
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = default!;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
```

## File: Entities/UserFollow.cs
```csharp
namespace Sportiva.Entities;

public class UserFollow
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    public string FollowerId { get; set; } = string.Empty;  // اللي بيعمل follow
    public ApplicationUser Follower { get; set; } = default!;

    public string FollowingId { get; set; } = string.Empty; // اللي اتعمله follow
    public ApplicationUser Following { get; set; } = default!;

    public DateTime FollowedAt { get; set; } = DateTime.UtcNow;
}
```

## File: Entities/UserProfile.cs
```csharp
namespace Sportiva.Entities;

public class UserProfile
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = default!;
    public string? Bio { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public SportType? PreferredSport { get; set; }
    public string? PreferredCity { get; set; }
    public bool IsDeleted { get; set; } = false;
}
```

## File: Enums/BookingStatus.cs
```csharp
namespace Sportiva.Enums;

public enum BookingStatus
{
    Pending,
    Confirmed,
    Cancelled,
    Completed
}
```

## File: Enums/JoinRequestStatus.cs
```csharp
namespace Sportiva.Enums;

public enum JoinRequestStatus
{
    Pending,
    Accepted,
    Rejected
}
```

## File: Enums/MatchStatus.cs
```csharp
namespace Sportiva.Enums;

public enum MatchStatus
{
    Open,
    Full,
    InProgress,
    Completed,
    Cancelled
}
```

## File: Enums/NotificationPriority.cs
```csharp
namespace Sportiva.Enums;

public enum NotificationPriority
{
    Low,
    Normal,
    High
}
```

## File: Enums/NotificationType.cs
```csharp
namespace Sportiva.Enums;

public enum NotificationType
{
    NewFollower,
    PostLiked,
    PostCommented,
    CommentReplied,
    CommentReacted,
    BookingConfirmed,
    BookingCancelled,
    BookingReminder,
    MatchJoinRequestReceived,
    MatchJoinRequestAccepted,
    MatchJoinRequestRejected,
    MatchFull,
    NewMessage,
    SecurityAlert,
    GeneralInfo
}
```

## File: Enums/PaymentStatus.cs
```csharp
namespace Sportiva.Enums;

public enum PaymentStatus
{
    Pending,
    Paid,
    Failed,
    Refunded
}
```

## File: Enums/RequestStatus.cs
```csharp
namespace Sportiva.Enums;

public enum RequestStatus
{
    Pending,
    Approved,
    Rejected
}
```

## File: Enums/SportType.cs
```csharp
namespace Sportiva.Enums;

public enum SportType
{
    Football,
    Basketball,

    Tennis,
    Padel,
    Volleyball,
    Other
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

## File: Errors/ProfileErrors.cs
```csharp
namespace Sportiva.Errors;

public record ProfileErrors
{
    public static readonly Error Error =
        new("Profile.Error", "An error occurred while processing the profile", StatusCodes.Status500InternalServerError);

    public static readonly Error ProfileNotFound =
        new("Profile.NotFound", "User profile not found", StatusCodes.Status404NotFound);

    public static readonly Error CannotFollowSelf =
        new("Profile.CannotFollowSelf", "You cannot follow yourself", StatusCodes.Status400BadRequest);

    public static readonly Error AlreadyFollowing =
        new("Profile.AlreadyFollowing", "You are already following this user", StatusCodes.Status409Conflict);
}
```

## File: Errors/UserErrors.cs
```csharp
namespace Sportiva.Errors;

public record UserErrors
{
    public static readonly Error InvalidCredentials =
        new("User.InvalidCredentials", "Invalid email/password", StatusCodes.Status401Unauthorized);

    public static readonly Error DisabledUser =
        new("User.DisabledUser", "Disabled user, please contact your administrator", StatusCodes.Status401Unauthorized);

    public static readonly Error LockedUser =
        new("User.LockedUser", "Locked user, please contact your administrator", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidJwtToken =
        new("User.InvalidJwtToken", "Invalid Jwt token", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidRefreshToken =
        new("User.InvalidRefreshToken", "Invalid refresh token", StatusCodes.Status401Unauthorized);

    public static readonly Error DuplicatedEmail =
        new("User.DuplicatedEmail", "Another user with the same email is already exists", StatusCodes.Status409Conflict);

    public static readonly Error EmailNotConfirmed =
        new("User.EmailNotConfirmed", "Email is not confirmed", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidCode =
        new("User.InvalidCode", "Invalid code", StatusCodes.Status401Unauthorized);

    public static readonly Error DuplicatedConfirmation =
        new("User.DuplicatedConfirmation", "Email already confirmed", StatusCodes.Status400BadRequest);

    public static readonly Error UserNotFound =
    new("User.UserNotFound", "User is not found", StatusCodes.Status404NotFound);

    public static readonly Error InvalidRoles =
        new("Role.InvalidRoles", "Invalid roles", StatusCodes.Status400BadRequest);
    public static readonly Error DuplicateProfile =
        new("User.DuplicateProfile", "User profile already exists", StatusCodes.Status409Conflict);
    public static readonly Error ProfileNotFound =
        new("User.ProfileNotFound", "User profile not found", StatusCodes.Status404NotFound);
    public static readonly Error FileNotFound =
        new("User.FileNotFound", "File not found", StatusCodes.Status404NotFound);

    public static readonly Error DuplicatedRequest
        = new("User.DuplicatedRequest", "A similar request is already in progress, please wait", StatusCodes.Status429TooManyRequests);
    public static readonly Error RequestNotFound =
        new("User.RequestNotFound", "Request not found", StatusCodes.Status404NotFound);
    public static readonly Error CannotRejectRequest =
        new("User.CannotRejectRequest", "Cannot reject a  request", StatusCodes.Status400BadRequest);
    public static readonly Error CannotApproveRequest =
        new("User.CannotApproveRequest", "Cannot approve a request", StatusCodes.Status400BadRequest);
    public static readonly Error UnexpectedError =
        new("User.UnexpectedError", "An unexpected error occurred, please try again later", StatusCodes.Status500InternalServerError);
    public static readonly Error EmailNotFound =
        new("User.EmailNotFound", "Applicant email not found", StatusCodes.Status404NotFound);
    public static readonly Error NotFound =
        new("User.NotFound", "User is not found ", StatusCodes.Status404NotFound);

    public static readonly Error InvalidExternalLogin =
        new("User.InvalidExternalLogin", "The External Login is Invalid", StatusCodes.Status401Unauthorized);

}
```

## File: Extensions/QueryableExtensions.cs
```csharp
using Sportiva.Contracts.Common;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace Sportiva.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> query, RequestFilters filters, Expression<Func<T, bool>>? searchPredicate = null, IEnumerable<string>? allowedSortColumns = null)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(filters);

        if (!string.IsNullOrWhiteSpace(filters.SearchValue) && searchPredicate is not null)
            query = query.Where(searchPredicate);

        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            var column = filters.SortColumn.Trim();

            if (allowedSortColumns is not null && !allowedSortColumns.Contains(column, StringComparer.OrdinalIgnoreCase))
                throw new ArgumentException($"Sort column '{column}' is not allowed.");

            var direction = filters.SortDirection == SortDirection.Desc ? "DESC" : "ASC";
            query = query.OrderBy($"{column} {direction}");
        }

        return query;
    }

    public static Task<PaginatedList<T>> ToPaginatedListAsync<T>(this IQueryable<T> query, RequestFilters filters, CancellationToken cancellationToken = default)
        => PaginatedList<T>.CreateAsync(query, filters.PageNumber, filters.PageSize, cancellationToken);
}



//private static readonly string[] JobSortColumns = ["Title", "CreatedAt", "Salary"];

//    var jobs = await _context.Jobs
//        .Where(j => j.CompanyId == companyId)
//        .ApplyFilters(
//            filters,
//            searchPredicate: x => x.Title.Contains(filters.SearchValue!),
//            allowedSortColumns: JobSortColumns)
//        .ProjectToType<JobResponse>()
//        .AsNoTracking()
//        .ToPaginatedListAsync(filters, cancellationToken);
```

## File: Extensions/UserExtensions.cs
```csharp
namespace Sportiva.Extensions;

public static class UserExtensions
{
    public static string? GetUserId(this ClaimsPrincipal user) =>
       user.FindFirstValue(ClaimTypes.NameIdentifier);
    public static string? GetFirstName(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.GivenName);

    public static string? GetLastName(this ClaimsPrincipal user) =>
        user.FindFirstValue(JwtRegisteredClaimNames.FamilyName);

    public static string? GetFullName(this ClaimsPrincipal user) =>
        user.FindFirstValue(JwtRegisteredClaimNames.Nickname);

    public static string? GetEmail(this ClaimsPrincipal user) =>
        user.FindFirstValue(JwtRegisteredClaimNames.Email);
}
```

## File: GlobalUsings.cs
```csharp
global using FluentValidation;
global using Mapster;
global using MapsterMapper;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Diagnostics;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using Microsoft.AspNetCore.Identity.UI.Services;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.WebUtilities;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Tokens;
global using Sportiva.Abstractions;
global using Sportiva.Abstractions.Consts;
global using Sportiva.Authentication;
global using Sportiva.Authentication.Filters;
global using Sportiva.Contracts.Authentication;
global using Sportiva.Entities;
global using Sportiva.Enums;
global using Sportiva.Errors;
global using Sportiva.Helpers;
global using Sportiva.Persistence;
global using Sportiva.Settings;
//global using Sportiva.Specifications;
//global using Sportiva.Authentication.Filters;
global using System.ComponentModel.DataAnnotations;
global using System.IdentityModel.Tokens.Jwt;
global using System.Reflection;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;


















//https://www.google.com/maps/place/27%C2%B010'43.4%22N+31%C2%B009'37.7%22E/@27.1787074,31.1578847,17z/data=!3m1!4b1!4m4!3m3!8m2!3d27.1787074!4d31.1604596?hl=en&entry=ttu&g_ep=EgoyMDI2MDQwNy4wIKXMDSoASAFQAw%3D%3D
//هي دي الميثود كاملة:
//csharppublic static double Haversine(double lat1, double lon1, double lat2, double lon2)
//{
//    double R = 6371; // نصف قطر الأرض بالكيلومتر

//    double dLat = (lat2 - lat1) * Math.PI / 180;
//    double dLon = (lon2 - lon1) * Math.PI / 180;

//    double a =
//        Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
//        Math.Cos(lat1 * Math.PI / 180) *
//        Math.Cos(lat2 * Math.PI / 180) *
//        Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

//    double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

//    return Math.Round(R * c, 2); // المسافة بالكيلومتر لحدين عشري
//}
//مثال استخدام:
//csharpdouble userLat = 30.0444;
//double userLng = 31.2357;

//double stadiumLat = 27.1787074;
//double stadiumLng = 31.1578847;

//double distance = Haversine(userLat, userLng, stadiumLat, stadiumLng);

//Console.WriteLine($"المسافة: {distance} كم");
//لو عندك List ملاعب وعايز ترتبهم من الأقرب:
//csharpvar sortedStadiums = stadiums
//    .Select(s => new
//    {
//        Stadium = s,
//        Distance = Haversine(userLat, userLng, s.Latitude, s.Longitude)
//    })
//    .OrderBy(x => x.Distance)
//    .ToList();
```

## File: Helpers/EmailBodyBuilder.cs
```csharp
namespace Sportiva.Helpers
{
    public static class EmailBodyBuilder
    {
        private const string VerificationTemplate = @"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <meta name=""x-apple-disable-message-reformatting"" />
    <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
    <meta name=""color-scheme"" content=""light dark"" />
    <meta name=""supported-color-schemes"" content=""light dark"" />
    <title></title>
    <style type=""text/css"" rel=""stylesheet"" media=""all"">
        /* Base ------------------------------ */

        @import url(""https://fonts.googleapis.com/css?family=Nunito+Sans:400,700&display=swap"");
        body {
          width: 100% !important;
          height: 100%;
          margin: 0;
          -webkit-text-size-adjust: none;
        }

        a {
          color: #3869d4;
        }

        a img {
          border: none;
        }

        td {
          word-break: break-word;
        }

        .preheader {
          display: none !important;
          visibility: hidden;
          mso-hide: all;
          font-size: 1px;
          line-height: 1px;
          max-height: 0;
          max-width: 0;
          opacity: 0;
          overflow: hidden;
        }
        /* Type ------------------------------ */

        body,
        td,
        th {
          font-family: ""Nunito Sans"", Helvetica, Arial, sans-serif;
        }

        h1 {
          margin-top: 0;
          color: #333333;
          font-size: 22px;
          font-weight: bold;
          text-align: left;
        }

        h2 {
          margin-top: 0;
          color: #333333;
          font-size: 16px;
          font-weight: bold;
          text-align: left;
        }

        h3 {
          margin-top: 0;
          color: #333333;
          font-size: 14px;
          font-weight: bold;
          text-align: left;
        }

        td,
        th {
          font-size: 16px;
        }

        p,
        ul,
        ol,
        blockquote {
          margin: 0.4em 0 1.1875em;
          font-size: 16px;
          line-height: 1.625;
        }

        p.sub {
          font-size: 13px;
        }
        /* Utilities ------------------------------ */

        .align-right {
          text-align: right;
        }

        .align-left {
          text-align: left;
        }

        .align-center {
          text-align: center;
        }

        .u-margin-bottom-none {
          margin-bottom: 0;
        }
        /* Buttons ------------------------------ */

        .button {
          background-color: #3869d4;
          border-top: 10px solid #3869d4;
          border-right: 18px solid #3869d4;
          border-bottom: 10px solid #3869d4;
          border-left: 18px solid #3869d4;
          display: inline-block;
          color: #fff;
          text-decoration: none;
          border-radius: 3px;
          box-shadow: 0 2px 3px rgba(0, 0, 0, 0.16);
          -webkit-text-size-adjust: none;
          box-sizing: border-box;
        }

        .button--green {
          background-color: #22bc66;
          border-top: 10px solid #22bc66;
          border-right: 18px solid #22bc66;
          border-bottom: 10px solid #22bc66;
          border-left: 18px solid #22bc66;
        }

        .button--red {
          background-color: #ff6136;
          border-top: 10px solid #ff6136;
          border-right: 18px solid #ff6136;
          border-bottom: 10px solid #ff6136;
          border-left: 18px solid #ff6136;
        }

        @media only screen and (max-width: 500px) {
          .button {
            width: 100% !important;
            text-align: center !important;
          }
        }
        /* Attribute list ------------------------------ */

        .attributes {
          margin: 0 0 21px;
        }

        .attributes_content {
          background-color: #f4f4f7;
          padding: 16px;
        }

        .attributes_item {
          padding: 0;
        }
        /* Related Items ------------------------------ */

        .related {
          width: 100%;
          margin: 0;
          padding: 25px 0 0 0;
          -premailer-width: 100%;
          -premailer-cellpadding: 0;
          -premailer-cellspacing: 0;
        }

        .related_item {
          padding: 10px 0;
          color: #cbcccf;
          font-size: 15px;
          line-height: 18px;
        }

        .related_item-title {
          display: block;
          margin: 0.5em 0 0;
        }

        .related_item-thumb {
          display: block;
          padding-bottom: 10px;
        }

        .related_heading {
          border-top: 1px solid #cbcccf;
          text-align: center;
          padding: 25px 0 10px;
        }
        /* Discount Code ------------------------------ */

        .discount {
          width: 100%;
          margin: 0;
          padding: 24px;
          -premailer-width: 100%;
          -premailer-cellpadding: 0;
          -premailer-cellspacing: 0;
          background-color: #f4f4f7;
          border: 2px dashed #cbcccf;
        }

        .discount_heading {
          text-align: center;
        }

        .discount_body {
          text-align: center;
          font-size: 15px;
        }
        /* Social Icons ------------------------------ */

        .social {
          width: auto;
        }

        .social td {
          padding: 0;
          width: auto;
        }

        .social_icon {
          height: 20px;
          margin: 0 8px 10px 8px;
          padding: 0;
        }
        /* Data table ------------------------------ */

        .purchase {
          width: 100%;
          margin: 0;
          padding: 35px 0;
          -premailer-width: 100%;
          -premailer-cellpadding: 0;
          -premailer-cellspacing: 0;
        }

        .purchase_content {
          width: 100%;
          margin: 0;
          padding: 25px 0 0 0;
          -premailer-width: 100%;
          -premailer-cellpadding: 0;
          -premailer-cellspacing: 0;
        }

        .purchase_item {
          padding: 10px 0;
          color: #51545e;
          font-size: 15px;
          line-height: 18px;
        }

        .purchase_heading {
          padding-bottom: 8px;
          border-bottom: 1px solid #eaeaec;
        }

        .purchase_heading p {
          margin: 0;
          color: #85878e;
          font-size: 12px;
        }

        .purchase_footer {
          padding-top: 15px;
          border-top: 1px solid #eaeaec;
        }

        .purchase_total {
          margin: 0;
          text-align: right;
          font-weight: bold;
          color: #333333;
        }

        .purchase_total--label {
          padding: 0 15px 0 0;
        }

        body {
          background-color: #f2f4f6;
          color: #51545e;
        }

        p {
          color: #51545e;
        }

        .email-wrapper {
          width: 100%;
          margin: 0;
          padding: 0;
          -premailer-width: 100%;
          -premailer-cellpadding: 0;
          -premailer-cellspacing: 0;
          background-color: #f2f4f6;
        }

        .email-content {
          width: 100%;
          margin: 0;
          padding: 0;
          -premailer-width: 100%;
          -premailer-cellpadding: 0;
          -premailer-cellspacing: 0;
        }
        /* Masthead ----------------------- */

        .email-masthead {
          padding: 25px 0;
          text-align: center;
        }

        .email-masthead_logo {
          width: 94px;
        }

        .email-masthead_name {
          font-size: 16px;
          font-weight: bold;
          color: #a8aaaf;
          text-decoration: none;
          text-shadow: 0 1px 0 white;
        }
        /* Body ------------------------------ */

        .email-body {
          width: 100%;
          margin: 0;
          padding: 0;
          -premailer-width: 100%;
          -premailer-cellpadding: 0;
          -premailer-cellspacing: 0;
        }

        .email-body_inner {
          width: 570px;
          margin: 0 auto;
          padding: 0;
          -premailer-width: 570px;
          -premailer-cellpadding: 0;
          -premailer-cellspacing: 0;
          background-color: #ffffff;
        }

        .email-footer {
          width: 570px;
          margin: 0 auto;
          padding: 0;
          -premailer-width: 570px;
          -premailer-cellpadding: 0;
          -premailer-cellspacing: 0;
          text-align: center;
        }

        .email-footer p {
          color: #a8aaaf;
        }

        .body-action {
          width: 100%;
          margin: 30px auto;
          padding: 0;
          -premailer-width: 100%;
          -premailer-cellpadding: 0;
          -premailer-cellspacing: 0;
          text-align: center;
        }

        .body-sub {
          margin-top: 25px;
          padding-top: 25px;
          border-top: 1px solid #eaeaec;
        }

        .content-cell {
          padding: 45px;
        }
        /*Media Queries ------------------------------ */

        @media only screen and (max-width: 600px) {
          .email-body_inner,
          .email-footer {
            width: 100% !important;
          }
        }

        @media (prefers-color-scheme: dark) {
          body,
          .email-body,
          .email-body_inner,
          .email-content,
          .email-wrapper,
          .email-masthead,
          .email-footer {
            background-color: #333333 !important;
            color: #fff !important;
          }
          p,
          ul,
          ol,
          blockquote,
          h1,
          h2,
          h3,
          span,
          .purchase_item {
            color: #fff !important;
          }
          .attributes_content,
          .discount {
            background-color: #222 !important;
          }
          .email-masthead_name {
            text-shadow: none !important;
          }
        }

        :root {
          color-scheme: light dark;
          supported-color-schemes: light dark;
        }
    </style>
    <!--[if mso]>
      <style type=""text/css"">
        .f-fallback {
          font-family: Arial, sans-serif;
        }
      </style>
    <![endif]-->
</head>
<body>
    <span class=""preheader"">
        Use this link to reset your password. The link is only valid for 24
        hours.
    </span>
    <table class=""email-wrapper""
           width=""100%""
           cellpadding=""0""
           cellspacing=""0""
           role=""presentation"">
        <tr>
            <td align=""center"">
                <table class=""email-content""
                       width=""100%""
                       cellpadding=""0""
                       cellspacing=""0""
                       role=""presentation"">
                    <tr>
                        <td class=""email-masthead"">
                            <a href=""https://example.com""
                               class=""f-fallback email-masthead_name"">
                                Career Path
                            </a>
                        </td>
                    </tr>
                    <!-- Email Body -->
                    <tr>
                        <td class=""email-body""
                            width=""570""
                            cellpadding=""0""
                            cellspacing=""0"">
                            <table class=""email-body_inner""
                                   align=""center""
                                   width=""570""
                                   cellpadding=""0""
                                   cellspacing=""0""
                                   role=""presentation"">
                                <!-- Body content -->
                                <tr>
                                    <td class=""content-cell"">
                                        <div class=""f-fallback"">
                                            <h1>Hi {{name}},</h1>
                                            <p>
                                                To confirm your email, please click the button below:
                                            </p>
                                            <!-- Action -->
                                            <table class=""body-action""
                                                   align=""center""
                                                   width=""100%""
                                                   cellpadding=""0""
                                                   cellspacing=""0""
                                                   role=""presentation"">
                                                <tr>
                                                    <td align=""center"">
                                                        <!-- Border based button
                                                        https://litmus.com/blog/a-guide-to-bulletproof-buttons-in-email-design -->
                                                        <table width=""100%""
                                                               border=""0""
                                                               cellspacing=""0""
                                                               cellpadding=""0""
                                                               role=""presentation"">
                                                            <tr>
                                                                <td align=""center"">
                                                                    <a href=""{{action_url}}""
                                                                       class=""f-fallback button button--green""
                                                                       target=""_blank"">Verify email address</a>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                            <p>Thanks, <br />Career Path team</p>
                                            <!-- Sub copy -->
                                            <table class=""body-sub"" role=""presentation"">
                                                <tr>
                                                    <td>
                                                        <p class=""f-fallback sub"">
                                                            If you’re having trouble with the button above,
                                                            copy and paste the URL below into your web
                                                            browser.
                                                        </p>
                                                        <p class=""f-fallback sub"">{{action_url}}</p>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table class=""email-footer""
                                   align=""center""
                                   width=""570""
                                   cellpadding=""0""
                                   cellspacing=""0""
                                   role=""presentation"">
                                <tr>
                                    <td class=""content-cell"" align=""center"">
                                        <p class=""f-fallback sub align-center"">
                                            Career Path
                                            <br />1234 Street Rd. <br />Cairo, Egypt
                                        </p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        public static string GenerateEmailBody(Dictionary<string, string> emailBody)
        {
            string temp = VerificationTemplate;

            foreach (var item in emailBody)
            {
                temp = temp.Replace(item.Key, item.Value);
            }

            return temp;
        }
    }
}
```

## File: Helpers/FileHelper.cs
```csharp
namespace Sportiva.Helpers
{
    public class FileHelper
    {
        public async static Task<string?> UploadeFileAsync(IFormFile file, string location, IWebHostEnvironment env, IHttpContextAccessor accessor)
        {
            if (file is null)
                return null;
            var path = Path.Combine(env.WebRootPath, location);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var extention = Path.GetExtension(file.FileName);
            var fileName = Guid.NewGuid().ToString().Replace("-", string.Empty);

            var fullPath = Path.Combine(path, fileName + extention);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);

            }
            var origin = accessor.HttpContext?.Request;

            return $"{origin.Scheme}://{origin.Host}/{location}/{fileName}{extention}";
        }

        //DeleteFile عمليه سريعه جدا علي الكورس
        public static void DeleteFile(string oldPath, string location, IWebHostEnvironment env)
        {
            if (string.IsNullOrEmpty(oldPath))
                return;

            var fileName = Path.GetFileName(new Uri(oldPath).LocalPath);
            var path = Path.Combine(env.WebRootPath, location, fileName);

            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
```

## File: Helpers/ForgetPasswordBodyBuilder.cs
```csharp
namespace Sportiva.Helpers
{
    public static class ForgetPasswordBodyBuilder
    {
        private const string ForgotPasswordTemplate = @"
<!DOCTYPE html>
<html lang=""en"">
  <head>
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <meta name=""x-apple-disable-message-reformatting"" />
    <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
    <meta name=""color-scheme"" content=""light dark"" />
    <meta name=""supported-color-schemes"" content=""light dark"" />
    <title></title>
    <style type=""text/css"" rel=""stylesheet"" media=""all"">
      /* Base ------------------------------ */

      @import url(""https://fonts.googleapis.com/css?family=Nunito+Sans:400,700&display=swap"");
      body {
        width: 100% !important;
        height: 100%;
        margin: 0;
        -webkit-text-size-adjust: none;
      }

      a {
        color: #3869d4;
      }

      a img {
        border: none;
      }

      td {
        word-break: break-word;
      }

      .preheader {
        display: none !important;
        visibility: hidden;
        mso-hide: all;
        font-size: 1px;
        line-height: 1px;
        max-height: 0;
        max-width: 0;
        opacity: 0;
        overflow: hidden;
      }
      /* Type ------------------------------ */

      body,
      td,
      th {
        font-family: ""Nunito Sans"", Helvetica, Arial, sans-serif;
      }

      h1 {
        margin-top: 0;
        color: #333333;
        font-size: 22px;
        font-weight: bold;
        text-align: left;
      }

      h2 {
        margin-top: 0;
        color: #333333;
        font-size: 16px;
        font-weight: bold;
        text-align: left;
      }

      h3 {
        margin-top: 0;
        color: #333333;
        font-size: 14px;
        font-weight: bold;
        text-align: left;
      }

      td,
      th {
        font-size: 16px;
      }

      p,
      ul,
      ol,
      blockquote {
        margin: 0.4em 0 1.1875em;
        font-size: 16px;
        line-height: 1.625;
      }

      p.sub {
        font-size: 13px;
      }
      /* Utilities ------------------------------ */

      .align-right {
        text-align: right;
      }

      .align-left {
        text-align: left;
      }

      .align-center {
        text-align: center;
      }

      .u-margin-bottom-none {
        margin-bottom: 0;
      }
      /* Buttons ------------------------------ */

      .button {
        background-color: #3869d4;
        border-top: 10px solid #3869d4;
        border-right: 18px solid #3869d4;
        border-bottom: 10px solid #3869d4;
        border-left: 18px solid #3869d4;
        display: inline-block;
        color: #fff;
        text-decoration: none;
        border-radius: 3px;
        box-shadow: 0 2px 3px rgba(0, 0, 0, 0.16);
        -webkit-text-size-adjust: none;
        box-sizing: border-box;
      }

      .button--green {
        background-color: #22bc66;
        border-top: 10px solid #22bc66;
        border-right: 18px solid #22bc66;
        border-bottom: 10px solid #22bc66;
        border-left: 18px solid #22bc66;
      }

      .button--red {
        background-color: #ff6136;
        border-top: 10px solid #ff6136;
        border-right: 18px solid #ff6136;
        border-bottom: 10px solid #ff6136;
        border-left: 18px solid #ff6136;
      }

      @media only screen and (max-width: 500px) {
        .button {
          width: 100% !important;
          text-align: center !important;
        }
      }
      /* Attribute list ------------------------------ */

      .attributes {
        margin: 0 0 21px;
      }

      .attributes_content {
        background-color: #f4f4f7;
        padding: 16px;
      }

      .attributes_item {
        padding: 0;
      }
      /* Related Items ------------------------------ */

      .related {
        width: 100%;
        margin: 0;
        padding: 25px 0 0 0;
        -premailer-width: 100%;
        -premailer-cellpadding: 0;
        -premailer-cellspacing: 0;
      }

      .related_item {
        padding: 10px 0;
        color: #cbcccf;
        font-size: 15px;
        line-height: 18px;
      }

      .related_item-title {
        display: block;
        margin: 0.5em 0 0;
      }

      .related_item-thumb {
        display: block;
        padding-bottom: 10px;
      }

      .related_heading {
        border-top: 1px solid #cbcccf;
        text-align: center;
        padding: 25px 0 10px;
      }
      /* Discount Code ------------------------------ */

      .discount {
        width: 100%;
        margin: 0;
        padding: 24px;
        -premailer-width: 100%;
        -premailer-cellpadding: 0;
        -premailer-cellspacing: 0;
        background-color: #f4f4f7;
        border: 2px dashed #cbcccf;
      }

      .discount_heading {
        text-align: center;
      }

      .discount_body {
        text-align: center;
        font-size: 15px;
      }
      /* Social Icons ------------------------------ */

      .social {
        width: auto;
      }

      .social td {
        padding: 0;
        width: auto;
      }

      .social_icon {
        height: 20px;
        margin: 0 8px 10px 8px;
        padding: 0;
      }
      /* Data table ------------------------------ */

      .purchase {
        width: 100%;
        margin: 0;
        padding: 35px 0;
        -premailer-width: 100%;
        -premailer-cellpadding: 0;
        -premailer-cellspacing: 0;
      }

      .purchase_content {
        width: 100%;
        margin: 0;
        padding: 25px 0 0 0;
        -premailer-width: 100%;
        -premailer-cellpadding: 0;
        -premailer-cellspacing: 0;
      }

      .purchase_item {
        padding: 10px 0;
        color: #51545e;
        font-size: 15px;
        line-height: 18px;
      }

      .purchase_heading {
        padding-bottom: 8px;
        border-bottom: 1px solid #eaeaec;
      }

      .purchase_heading p {
        margin: 0;
        color: #85878e;
        font-size: 12px;
      }

      .purchase_footer {
        padding-top: 15px;
        border-top: 1px solid #eaeaec;
      }

      .purchase_total {
        margin: 0;
        text-align: right;
        font-weight: bold;
        color: #333333;
      }

      .purchase_total--label {
        padding: 0 15px 0 0;
      }

      body {
        background-color: #f2f4f6;
        color: #51545e;
      }

      p {
        color: #51545e;
      }

      .email-wrapper {
        width: 100%;
        margin: 0;
        padding: 0;
        -premailer-width: 100%;
        -premailer-cellpadding: 0;
        -premailer-cellspacing: 0;
        background-color: #f2f4f6;
      }

      .email-content {
        width: 100%;
        margin: 0;
        padding: 0;
        -premailer-width: 100%;
        -premailer-cellpadding: 0;
        -premailer-cellspacing: 0;
      }
      /* Masthead ----------------------- */

      .email-masthead {
        padding: 25px 0;
        text-align: center;
      }

      .email-masthead_logo {
        width: 94px;
      }

      .email-masthead_name {
        font-size: 16px;
        font-weight: bold;
        color: #a8aaaf;
        text-decoration: none;
        text-shadow: 0 1px 0 white;
      }
      /* Body ------------------------------ */

      .email-body {
        width: 100%;
        margin: 0;
        padding: 0;
        -premailer-width: 100%;
        -premailer-cellpadding: 0;
        -premailer-cellspacing: 0;
      }

      .email-body_inner {
        width: 570px;
        margin: 0 auto;
        padding: 0;
        -premailer-width: 570px;
        -premailer-cellpadding: 0;
        -premailer-cellspacing: 0;
        background-color: #ffffff;
      }

      .email-footer {
        width: 570px;
        margin: 0 auto;
        padding: 0;
        -premailer-width: 570px;
        -premailer-cellpadding: 0;
        -premailer-cellspacing: 0;
        text-align: center;
      }

      .body-action {
        width: 100%;
        margin: 30px auto;
        padding: 0;
        -premailer-width: 100%;
        -premailer-cellpadding: 0;
        -premailer-cellspacing: 0;
        text-align: center;
      }

      .body-sub {
        margin-top: 25px;
        padding-top: 25px;
        border-top: 1px solid #eaeaec;
      }

      .content-cell {
        padding: 45px;
      }
    </style>
  </head>
  <body>
    <span class=""preheader"">
      Use this link to reset your password. The link is only valid for 24 hours.
    </span>
    <table class=""email-wrapper"" width=""100%"" cellpadding=""0"" cellspacing=""0"" role=""presentation"">
      <tr>
        <td align=""center"">
          <table class=""email-content"" width=""100%"" cellpadding=""0"" cellspacing=""0"" role=""presentation"">
            <tr>
              <td class=""email-masthead"">
                <a href=""https://example.com"" class=""f-fallback email-masthead_name"">
                  Career Path
                </a>
              </td>
            </tr>
            <tr>
              <td class=""email-body"" width=""570"" cellpadding=""0"" cellspacing=""0"">
                <table class=""email-body_inner"" align=""center"" width=""570"" cellpadding=""0"" cellspacing=""0"" role=""presentation"">
                  <tr>
                    <td class=""content-cell"">
                      <div class=""f-fallback"">
                        <h1>Hi {{name}},</h1>
                        <p>You recently requested to reset your password for your Career Path account. Use the button below to reset it.</p>
                        <table class=""body-action"" align=""center"" width=""100%"" cellpadding=""0"" cellspacing=""0"" role=""presentation"">
                          <tr>
                            <td align=""center"">
                              <table width=""100%"" border=""0"" cellspacing=""0"" cellpadding=""0"" role=""presentation"">
                                <tr>
                                  <td align=""center"">
                                    <a href=""{{action_url}}"" class=""f-fallback button button--green"" target=""_blank"">Reset your password</a>
                                  </td>
                                </tr>
                              </table>
                            </td>
                          </tr>
                        </table>
                        <p>Thanks, <br />The Career Path team</p>
                        <table class=""body-sub"" role=""presentation"">
                          <tr>
                            <td>
                              <p class=""f-fallback sub"">If you’re having trouble with the button above, copy and paste the URL below into your web browser.</p>
                              <p class=""f-fallback sub"">{{action_url}}</p>
                            </td>
                          </tr>
                        </table>
                      </div>
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
            <tr>
              <td>
                <table class=""email-footer"" align=""center"" width=""570"" cellpadding=""0"" cellspacing=""0"" role=""presentation"">
                  <tr>
                    <td class=""content-cell"" align=""center"">
                      <p class=""f-fallback sub align-center"">
                        Career Path
                        <br />1234 Street Rd. <br />Cairo, Egypt
                      </p>
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
          </table>
        </td>
      </tr>
    </table>
  </body>
</html>";

        public static string GenerateEmailBody(Dictionary<string, string> emailBody)
        {
            string temp = ForgotPasswordTemplate;

            foreach (var item in emailBody)
            {
                temp = temp.Replace(item.Key, item.Value);
            }

            return temp;
        }
    }
}
```

## File: Mapping/MappingConfigurations.cs
```csharp
namespace Sportiva.Mapping;

public class MappingConfigurations : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegisterRequest, ApplicationUser>()
            .Map(dest => dest.UserName, src => src.Email);

        //config.NewConfig<UserProfile, ProfileResponse>()
        //    .Map(dest => dest.FullName, src => src.User.FullName);
        // ✅ ضيف الـ tuple mapping
        //config.NewConfig<(ApplicationUser User, IEnumerable<string> Roles), UserResponse>()
        //    .Map(dest => dest.Id, src => src.User.Id)
        //    .Map(dest => dest.FirstName, src => src.User.FirstName)
        //    .Map(dest => dest.LastName, src => src.User.LastName)
        //    .Map(dest => dest.Email, src => src.User.Email)
        //    .Map(dest => dest.IsDisabled, src => src.User.IsDisabled)
        //    .Map(dest => dest.Roles, src => src.Roles);



    }
}
```

## File: Persistence/EntitiesConfigurations/ClubConfiguration.cs
```csharp
namespace Sportiva.Persistence.EntitiesConfigurations;

public class ClubConfiguration : IEntityTypeConfiguration<Club>
{
    public void Configure(EntityTypeBuilder<Club> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .HasMaxLength(200);

        builder.Property(x => x.Governorate).HasMaxLength(100);
        builder.Property(x => x.City).HasMaxLength(100);
        builder.Property(x => x.Address).HasMaxLength(300);
        builder.Property(x => x.PhoneNumber).HasMaxLength(20);
        builder.Property(x => x.Email).HasMaxLength(150);
        builder.Property(x => x.LogoUrl).HasMaxLength(500);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasMany(x => x.Courts)
               .WithOne(c => c.Club)
               .HasForeignKey(c => c.ClubId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Subscriptions)
               .WithOne(s => s.Club)
               .HasForeignKey(s => s.ClubId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
```

## File: Persistence/EntitiesConfigurations/ClubSubscriptionConfiguration.cs
```csharp
namespace Sportiva.Persistence.EntitiesConfigurations;

public class ClubSubscriptionConfiguration : IEntityTypeConfiguration<ClubSubscription>
{
    public void Configure(EntityTypeBuilder<ClubSubscription> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Ignore(x => x.IsActive); // Computed property

        builder.HasIndex(x => new { x.ClubId, x.EndDate });

        builder.HasOne(x => x.Plan)
               .WithMany(p => p.ClubSubscriptions)
               .HasForeignKey(x => x.PlanId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Payments)
               .WithOne(p => p.ClubSubscription)
               .HasForeignKey(p => p.ClubSubscriptionId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
```

## File: Persistence/EntitiesConfigurations/CourtConfiguration.cs
```csharp
namespace Sportiva.Persistence.EntitiesConfigurations;

public class CourtConfiguration : IEntityTypeConfiguration<Court>
{
    public void Configure(EntityTypeBuilder<Court> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .HasMaxLength(200);

        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.ImageUrl).HasMaxLength(500);

        builder.Property(x => x.PricePerHour)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(x => x.SportType)
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasMany(x => x.TimeSlots)
               .WithOne(t => t.Court)
               .HasForeignKey(t => t.CourtId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
```

## File: Persistence/EntitiesConfigurations/FriendlyMatchConfiguration.cs
```csharp
namespace Sportiva.Persistence.EntitiesConfigurations;

public class FriendlyMatchConfiguration : IEntityTypeConfiguration<FriendlyMatch>
{
    public void Configure(EntityTypeBuilder<FriendlyMatch> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.SportType)
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.Property(x => x.Status)
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.Property(x => x.Note).HasMaxLength(500);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasIndex(x => new { x.Date, x.Status });

        builder.HasOne(x => x.Organizer)
               .WithMany(u => u.OrganizedMatches)
               .HasForeignKey(x => x.OrganizerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Court)
               .WithMany()
               .HasForeignKey(x => x.CourtId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.JoinRequests)
               .WithOne(r => r.FriendlyMatch)
               .HasForeignKey(r => r.FriendlyMatchId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
```

## File: Persistence/EntitiesConfigurations/MatchJoinRequestConfiguration.cs
```csharp
namespace Sportiva.Persistence.EntitiesConfigurations;

public class MatchJoinRequestConfiguration : IEntityTypeConfiguration<MatchJoinRequest>
{
    public void Configure(EntityTypeBuilder<MatchJoinRequest> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
               .HasConversion<string>()
               .HasMaxLength(50);

        // لاعب واحد يقدر يطلب انضمام مرة واحدة لكل ماتش
        builder.HasIndex(x => new { x.FriendlyMatchId, x.PlayerId }).IsUnique();

        builder.HasOne(x => x.FriendlyMatch)
               .WithMany(m => m.JoinRequests)
               .HasForeignKey(x => x.FriendlyMatchId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Player)
               .WithMany(u => u.MatchJoinRequests)
               .HasForeignKey(x => x.PlayerId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
```

## File: Persistence/EntitiesConfigurations/MembershipUpgradeConfiguration.cs
```csharp
namespace Sportiva.Persistence.EntitiesConfigurations;

public class MembershipUpgradeConfiguration : IEntityTypeConfiguration<MembershipUpgrade>
{
    public void Configure(EntityTypeBuilder<MembershipUpgrade> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.Property(x => x.Note).HasMaxLength(500);

        builder.HasOne(x => x.User)
               .WithMany(u => u.MembershipUpgradeRequests)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
```

## File: Persistence/EntitiesConfigurations/PostLikeConfiguration.cs
```csharp
namespace Sportiva.Persistence.EntitiesConfigurations;

public class PostLikeConfiguration : IEntityTypeConfiguration<PostLike>
{
    public void Configure(EntityTypeBuilder<PostLike> builder)
    {
        builder.HasKey(x => x.Id);

        // يوزر واحد يعمل like واحد على كل post
        builder.HasIndex(x => new { x.PostId, x.UserId }).IsUnique();
        // builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne(x => x.Post)
               .WithMany(p => p.Likes)
               .HasForeignKey(x => x.PostId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
               .WithMany(u => u.PostLikes)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
```

## File: Persistence/EntitiesConfigurations/ReviewConfiguration.cs
```csharp
namespace Sportiva.Persistence.EntitiesConfigurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Rating)
               .IsRequired();

        // Rating لازم يكون بين 1 و 5
        builder.ToTable(t => t.HasCheckConstraint("CK_Review_Rating", "[Rating] >= 1 AND [Rating] <= 5"));

        builder.Property(x => x.Comment).HasMaxLength(1000);

        builder.HasQueryFilter(x => !x.IsDeleted);

        // يوزر واحد يعمل review واحد على كل booking
        builder.HasIndex(x => new { x.UserId, x.BookingId }).IsUnique();

        builder.HasOne(x => x.Court)
               .WithMany()
               .HasForeignKey(x => x.CourtId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Booking)
               .WithMany()
               .HasForeignKey(x => x.BookingId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
```

## File: Persistence/EntitiesConfigurations/SubscriptionPaymentConfiguration.cs
```csharp
namespace Sportiva.Persistence.EntitiesConfigurations;

public class SubscriptionPaymentConfiguration : IEntityTypeConfiguration<SubscriptionPayment>
{
    public void Configure(EntityTypeBuilder<SubscriptionPayment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Amount)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(x => x.Status)
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.Property(x => x.TransactionId).HasMaxLength(200);

        builder.HasOne(x => x.ClubSubscription)
               .WithMany(cs => cs.Payments)
               .HasForeignKey(x => x.ClubSubscriptionId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
```

## File: Persistence/EntitiesConfigurations/SubscriptionPlanConfiguration.cs
```csharp
namespace Sportiva.Persistence.EntitiesConfigurations;

public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .HasMaxLength(100)
               .IsRequired();
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.Description).HasMaxLength(500);

        builder.Property(x => x.MonthlyPrice)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.HasMany(x => x.ClubSubscriptions)
               .WithOne(s => s.Plan)
               .HasForeignKey(s => s.PlanId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
```

## File: Persistence/EntitiesConfigurations/TimeSlotConfiguration.cs
```csharp
namespace Sportiva.Persistence.EntitiesConfigurations;

public class TimeSlotConfiguration : IEntityTypeConfiguration<TimeSlot>
{
    public void Configure(EntityTypeBuilder<TimeSlot> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Day).IsRequired();
        builder.Property(x => x.StartTime).IsRequired();
        builder.Property(x => x.EndTime).IsRequired();

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasIndex(x => new { x.CourtId, x.Day });

        builder.HasMany(x => x.Bookings)
               .WithOne(b => b.TimeSlot)
               .HasForeignKey(b => b.TimeSlotId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
```

## File: Properties/launchSettings.json
```json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "http://localhost:5250",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "https://localhost:7283;http://localhost:5250",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

## File: README.md
```markdown
# SportivaForDepi
```

## File: Services/Abstraction/IAuthService.cs
```csharp
namespace Sportiva.Services;

public interface IAuthService
{
    Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request);
    Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request);
    Task<Result> SendResetPasswordCodeAsync(string email);
    Task<Result> ResetPasswordAsync(ResetPasswordRequest request);
    Task<Result<AuthResponse>> HandleGoogleLoginAsync();
    Task<Result<AuthResponse>> HandleGitHubLoginAsync();
}
```

## File: Services/Abstraction/ICommentService.cs
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

## File: Services/Abstraction/IMatchJoinRequestService.cs
```csharp
using Sportiva.Contracts.Common;
using Sportiva.Contracts.Matches;
using Sportiva.Enums;

namespace Sportiva.Services;

public interface IMatchJoinRequestService
{
    Task<Result<MatchJoinRequestResponse>> RequestToJoinAsync(
        string userId, string matchId, JoinMatchRequest request, CancellationToken ct = default);

    Task<Result<MatchJoinRequestResponse>> GetJoinRequestAsync(
        string userId, string matchId, string requestId, CancellationToken ct = default);

    Task<Result<PaginatedList<MatchJoinRequestResponse>>> GetMatchJoinRequestsAsync(
        string userId, string matchId, RequestFilters filters, CancellationToken ct = default);

    Task<Result<PaginatedList<MatchJoinRequestResponse>>> GetMyJoinRequestsAsync(
        string userId, RequestFilters filters, JoinRequestStatus? status = null, CancellationToken ct = default);

    Task<Result> AcceptJoinRequestAsync(
        string userId, string matchId, string requestId, CancellationToken ct = default);

    Task<Result> RejectJoinRequestAsync(
        string userId, string matchId, string requestId, CancellationToken ct = default);

    Task<Result> WithdrawJoinRequestAsync(
        string userId, string matchId, string requestId, CancellationToken ct = default);
}
```

## File: Services/Abstraction/IPostService.cs
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

## File: Services/Abstraction/IProfileService.cs
```csharp
using Sportiva.Contracts.Common;
using Sportiva.Contracts.Shared.Summaries;
using Sportiva.Contracts.Users;

namespace Sportiva.Services;

public interface IProfileService
{
    // ── Profile ──────────────────────────────────────────────────────
    Task<Result<UserProfileResponse>> GetProfileAsync(
        string profileUserId, string? currentUserId = null, CancellationToken ct = default);

    Task<Result<UserProfileResponse>> UpdateProfileInfoAsync(
     string userId, UpdateProfileInfoRequest request, CancellationToken ct = default);

    Task<Result<UserProfileResponse>> UpdateProfilePhotoAsync(
        string userId, UpdateProfilePhotoRequest request, CancellationToken ct = default);

    Task<Result<UserProfileResponse>> UpdateProfileCoverAsync(
        string userId, UpdateProfileCoverRequest request, CancellationToken ct = default);

    // ── Follow / Unfollow ────────────────────────────────────────────
    Task<Result<ToggleFollowResponse>> ToggleFollowAsync(
        string currentUserId, string targetUserId, CancellationToken ct = default);

    // ── Followers / Following ────────────────────────────────────────
    Task<PaginatedList<UserCardSummary>> GetFollowersAsync(
        string profileUserId, string? currentUserId, RequestFilters filters, CancellationToken ct = default);

    Task<PaginatedList<UserCardSummary>> GetFollowingAsync(
        string profileUserId, string? currentUserId, RequestFilters filters, CancellationToken ct = default);

    // ── Search ───────────────────────────────────────────────────────
    Task<PaginatedList<UserCardSummary>> SearchUsersAsync(
        string? currentUserId, RequestFilters filters, CancellationToken ct = default);
}
```

## File: Services/Implementation/AuthService.cs
```csharp
using Hangfire;
using System.Text.RegularExpressions;

namespace Sportiva.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IJwtProvider jwtProvider,
    ILogger<AuthService> logger,
    IEmailSender emailSender,
    IHttpContextAccessor httpContextAccessor,
    ApplicationDbContext context) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly ILogger<AuthService> _logger = logger;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ApplicationDbContext _context = context;
    private readonly string _frontendOrigin = "https://front-end-project-bay-seven.vercel.app";
    private readonly int _refreshTokenExpiryDays = 14;

    // ════════════════════════════════════════════════════════════════════════
    //  OAuth — Google / GitHub
    // ════════════════════════════════════════════════════════════════════════

    public async Task<Result<AuthResponse>> HandleGoogleLoginAsync()
        => await HandleExternalLoginAsync();

    public async Task<Result<AuthResponse>> HandleGitHubLoginAsync()
        => await HandleExternalLoginAsync();

    private async Task<Result<AuthResponse>> HandleExternalLoginAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();

            if (info is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidExternalLogin);

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);

            if (email is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidExternalLogin);

            var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty;
            var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? string.Empty;

            // GitHub doesn't provide GivenName/Surname — split the full name instead
            if (string.IsNullOrEmpty(firstName) && info.LoginProvider == "GitHub")
            {
                var fullName = info.Principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
                var nameParts = fullName.Split(' ', 2);
                firstName = nameParts.Length > 0 ? nameParts[0] : string.Empty;
                lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;
            }

            // ── Try signing in with the external login directly ──────────────
            var signInResult = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider,
                info.ProviderKey,
                isPersistent: false,
                bypassTwoFactor: true);

            ApplicationUser user;

            if (signInResult.Succeeded)
            {
                // ✅ Resolve the user id first, then load with RefreshTokens
                // (never call .Result inside a LINQ expression — it deadlocks)
                var linkedUser = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                if (linkedUser is null)
                    return Result.Failure<AuthResponse>(UserErrors.InvalidExternalLogin);

                user = await _context.Users
                    .Include(u => u.RefreshTokens)
                    .FirstOrDefaultAsync(u => u.Id == linkedUser.Id, cancellationToken);
            }
            else
            {
                // ── Find or create the user ──────────────────────────────────
                user = await _context.Users
                    .Include(u => u.RefreshTokens)
                    .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

                if (user is null)
                {
                    string username;

                    if (info.LoginProvider == "GitHub")
                    {
                        var githubUsername =
                            info.Principal.FindFirstValue("urn:github:login") ??
                            info.Principal.FindFirstValue("urn:github:name");

                        username = await GenerateUniqueUsernameAsync(email, githubUsername);
                    }
                    else
                    {
                        username = await GenerateUniqueUsernameAsync(email);
                    }

                    user = new ApplicationUser
                    {
                        UserName = username,
                        Email = email,
                        FirstName = firstName,
                        LastName = lastName,
                        EmailConfirmed = true   // OAuth provider already verified the email
                    };

                    var createResult = await _userManager.CreateAsync(user);

                    if (!createResult.Succeeded)
                    {
                        var createError = createResult.Errors.First();
                        return Result.Failure<AuthResponse>(
                            new Error(createError.Code, createError.Description,
                                      StatusCodes.Status400BadRequest));
                    }

                    await _userManager.AddToRoleAsync(user, DefaultRoles.Member.Name);

                    // Create the user profile — same as ConfirmEmailAsync
                    var userProfile = new UserProfile { UserId = user.Id };
                    _context.UserProfiles.Add(userProfile);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                await _userManager.AddLoginAsync(user, info);
            }

            // ── Common checks ────────────────────────────────────────────────
            if (user.IsDisabled)
                return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

            if (user.LockoutEnd > DateTime.UtcNow)
                return Result.Failure<AuthResponse>(UserErrors.LockedUser);

            // ── Issue JWT + refresh token ────────────────────────────────────
            var (userRoles, userPermissions) =
                await GetUserRolesAndPermissions(user, cancellationToken);

            var (token, expiresIn) = _jwtProvider.GenerateToken(user, userRoles, userPermissions);
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                ExpiresOn = refreshTokenExpiration
            });

            await _userManager.UpdateAsync(user);

            var response = new AuthResponse(
                user.Id, user.Email, user.FirstName, user.LastName,
                token, expiresIn, refreshToken, refreshTokenExpiration);

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during external login");
            return Result.Failure<AuthResponse>(UserErrors.UnexpectedError);
        }
    }

    private async Task<string> GenerateUniqueUsernameAsync(
        string email, string? preferredUsername = null)
    {
        if (!string.IsNullOrWhiteSpace(preferredUsername))
        {
            var sanitized = Regex.Replace(preferredUsername, @"[^a-zA-Z0-9_]", "");

            if (!string.IsNullOrWhiteSpace(sanitized))
            {
                if (await _userManager.FindByNameAsync(sanitized) is null)
                    return sanitized;

                var counter = 1;
                var candidate = $"{sanitized}{counter}";

                while (await _userManager.FindByNameAsync(candidate) is not null)
                    candidate = $"{sanitized}{++counter}";

                return candidate;
            }
        }

        var baseUsername = Regex.Replace(email.Split('@')[0], @"[^a-zA-Z0-9_]", "");

        if (string.IsNullOrWhiteSpace(baseUsername))
            baseUsername = "user";

        if (await _userManager.FindByNameAsync(baseUsername) is null)
            return baseUsername;

        var num = 1;
        var candidate2 = $"{baseUsername}{num}";

        while (await _userManager.FindByNameAsync(candidate2) is not null)
            candidate2 = $"{baseUsername}{++num}";

        return candidate2;
    }

    // ════════════════════════════════════════════════════════════════════════
    //  JWT Auth
    // ════════════════════════════════════════════════════════════════════════

    public async Task<Result<AuthResponse>> GetTokenAsync(
        string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

            if (user is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

            if (user.IsDisabled)
                return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

            var result = await _signInManager.PasswordSignInAsync(user, password, false, true);

            if (result.Succeeded)
            {
                var (userRoles, userPermissions) =
                    await GetUserRolesAndPermissions(user, cancellationToken);

                var (token, expiresIn) = _jwtProvider.GenerateToken(user, userRoles, userPermissions);
                var refreshToken = GenerateRefreshToken();
                var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

                user.RefreshTokens.Add(new RefreshToken
                {
                    Token = refreshToken,
                    ExpiresOn = refreshTokenExpiration
                });

                await _userManager.UpdateAsync(user);

                var response = new AuthResponse(
                    user.Id, user.Email, user.FirstName, user.LastName,
                    token, expiresIn, refreshToken, refreshTokenExpiration);

                return Result.Success(response);
            }

            var error = result.IsNotAllowed ? UserErrors.EmailNotConfirmed
                      : result.IsLockedOut ? UserErrors.LockedUser
                                             : UserErrors.InvalidCredentials;

            return Result.Failure<AuthResponse>(error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while generating token for user {Email}", email);
            return Result.Failure<AuthResponse>(UserErrors.UnexpectedError);
        }
    }

    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(
        string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = _jwtProvider.ValidateToken(token, validateLifetime: false);

            if (userId is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

            var user = await _context.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

            if (user.IsDisabled)
                return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

            if (user.LockoutEnd > DateTime.UtcNow)
                return Result.Failure<AuthResponse>(UserErrors.LockedUser);

            var userRefreshToken = user.RefreshTokens
                .SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

            if (userRefreshToken is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);

            userRefreshToken.RevokedOn = DateTime.UtcNow;

            var (userRoles, userPermissions) =
                await GetUserRolesAndPermissions(user, cancellationToken);

            var (newToken, expiresIn) = _jwtProvider.GenerateToken(user, userRoles, userPermissions);
            var newRefreshToken = GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                ExpiresOn = refreshTokenExpiration
            });

            await _userManager.UpdateAsync(user);

            var response = new AuthResponse(
                user.Id, user.Email, user.FirstName, user.LastName,
                newToken, expiresIn, newRefreshToken, refreshTokenExpiration);

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while refreshing token");
            return Result.Failure<AuthResponse>(UserErrors.UnexpectedError);
        }
    }

    public async Task<Result> RevokeRefreshTokenAsync(
        string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = _jwtProvider.ValidateToken(token, validateLifetime: false);

            if (userId is null)
                return Result.Failure(UserErrors.InvalidJwtToken);

            var user = await _context.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user is null)
                return Result.Failure(UserErrors.InvalidJwtToken);

            var userRefreshToken = user.RefreshTokens
                .SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

            if (userRefreshToken is null)
                return Result.Failure(UserErrors.InvalidRefreshToken);

            userRefreshToken.RevokedOn = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while revoking refresh token");
            return Result.Failure(UserErrors.UnexpectedError);
        }
    }

    // ════════════════════════════════════════════════════════════════════════
    //  Registration & Email Confirmation
    // ════════════════════════════════════════════════════════════════════════

    public async Task<Result> RegisterAsync(
        RegisterRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var emailIsExists = await _userManager.Users
                .AnyAsync(x => x.Email == request.Email, cancellationToken);

            if (emailIsExists)
                return Result.Failure(UserErrors.DuplicatedEmail);

            var user = request.Adapt<ApplicationUser>();
            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                _logger.LogInformation("Confirmation code: {code}", code);

                SendConfirmationEmail(user, code);
                return Result.Success();
            }

            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description,
                StatusCodes.Status400BadRequest));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while registering user {Email}", request.Email);
            return Result.Failure(UserErrors.UnexpectedError);
        }
    }

    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        try
        {
            if (await _userManager.FindByIdAsync(request.UserId) is not { } user)
                return Result.Failure(UserErrors.InvalidCode);

            if (user.EmailConfirmed)
                return Result.Failure(UserErrors.DuplicatedConfirmation);

            var code = request.Code;

            try
            {
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            }
            catch (FormatException)
            {
                return Result.Failure(UserErrors.InvalidCode);
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, DefaultRoles.Member.Name);
                var userProfile = new UserProfile { UserId = user.Id };
                _context.UserProfiles.Add(userProfile);
                await _context.SaveChangesAsync();
                return Result.Success();
            }

            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description,
                StatusCodes.Status400BadRequest));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while confirming email for user {UserId}",
                request.UserId);
            return Result.Failure(UserErrors.UnexpectedError);
        }
    }

    public async Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request)
    {
        try
        {
            if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
                return Result.Success();

            if (user.EmailConfirmed)
                return Result.Failure(UserErrors.DuplicatedConfirmation);

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            _logger.LogInformation("Confirmation code: {code}", code);

            SendConfirmationEmail(user, code);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occurred while resending confirmation email to {Email}", request.Email);
            return Result.Failure(UserErrors.UnexpectedError);
        }
    }

    // ════════════════════════════════════════════════════════════════════════
    //  Password Reset
    // ════════════════════════════════════════════════════════════════════════

    public async Task<Result> SendResetPasswordCodeAsync(string email)
    {
        try
        {
            if (await _userManager.FindByEmailAsync(email) is not { } user)
                return Result.Success();

            if (!user.EmailConfirmed)
                return Result.Failure(UserErrors.EmailNotConfirmed with
                {
                    StatusCode = StatusCodes.Status400BadRequest
                });

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            _logger.LogInformation("Reset code: {code}", code);

            SendResetPasswordEmail(user, code);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occurred while sending reset password code to {Email}", email);
            return Result.Failure(UserErrors.UnexpectedError);
        }
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null || !user.EmailConfirmed)
                return Result.Failure(UserErrors.InvalidCode);

            IdentityResult result;

            try
            {
                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
                result = await _userManager.ResetPasswordAsync(user, code, request.NewPassword);
            }
            catch (FormatException)
            {
                result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
            }

            if (result.Succeeded)
                return Result.Success();

            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description,
                StatusCodes.Status401Unauthorized));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occurred while resetting password for {Email}", request.Email);
            return Result.Failure(UserErrors.UnexpectedError);
        }
    }

    // ════════════════════════════════════════════════════════════════════════
    //  Private Helpers
    // ════════════════════════════════════════════════════════════════════════

    private static string GenerateRefreshToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    private void SendConfirmationEmail(ApplicationUser user, string code)
    {
        var emailBody = EmailBodyBuilder.GenerateEmailBody(
            new Dictionary<string, string>
            {
            { "{{name}}", user.FirstName },
            { "{{action_url}}", $"{_frontendOrigin}/auth/emailConfirmation?userId={user.Id}&code={code}" }
            });

        BackgroundJob.Enqueue(() =>
            _emailSender.SendEmailAsync(
                user.Email!,
                "✅ Career Path : Email Confirmation",
                emailBody
            ));
    }

    private void SendResetPasswordEmail(ApplicationUser user, string code)
    {
        var emailBody = ForgetPasswordBodyBuilder.GenerateEmailBody(
            new Dictionary<string, string>
            {
            { "{{name}}", user.FirstName },
            { "{{action_url}}", $"{_frontendOrigin}/auth/forgetPassword?email={user.Email}&code={code}" }
            });

        BackgroundJob.Enqueue(() =>
            _emailSender.SendEmailAsync(
                user.Email!,
                "✅ Career Path: Reset Password",
                emailBody
            ));
    }

    private async Task<(IEnumerable<string> roles, IEnumerable<string> permissions)>
        GetUserRolesAndPermissions(ApplicationUser user, CancellationToken cancellationToken)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        var userPermissions = await (
            from r in _context.Roles
            join p in _context.RoleClaims on r.Id equals p.RoleId
            where userRoles.Contains(r.Name!)
            select p.ClaimValue!)
            .Distinct()
            .ToListAsync(cancellationToken);

        return (userRoles, userPermissions);
    }
}
```

## File: Services/Implementation/CommentService.cs
```csharp
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
```

## File: Services/Implementation/EmailService.cs
```csharp
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
namespace Sportiva.Services;
public class EmailService(IOptions<MailSettings> mailSettings, ILogger<EmailService> logger) : IEmailSender
{
    private readonly MailSettings _mailSettings = mailSettings.Value;
    private readonly ILogger<EmailService> _logger = logger;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var message = new MimeMessage
        {
            Sender = MailboxAddress.Parse(_mailSettings.Mail),
            Subject = subject
        };

        message.To.Add(MailboxAddress.Parse(email));

        var builder = new BodyBuilder
        {
            HtmlBody = htmlMessage
        };

        message.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();

        _logger.LogInformation("Sending email to {email}", email);

        smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
        smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
        await smtp.SendAsync(message);
        smtp.Disconnect(true);
    }
}
```

## File: Services/Implementation/PostService.cs
```csharp
using Sportiva.Contracts.Common;
using Sportiva.Contracts.Posts;
using Sportiva.Contracts.Shared.Summaries;
using Sportiva.Extensions;

namespace Sportiva.Services;

public class PostService : IPostService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PostService> _logger;
    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _accessor;

    // الـ columns المسموح بالـ sort عليها فقط
    private static readonly string[] AllowedPostSortColumns = ["CreatedAt", "LikesCount"];
    private static readonly string[] AllowedLikerSortColumns = ["LikedAt"];

    public PostService(
        ApplicationDbContext context,
        ILogger<PostService> logger,
        IWebHostEnvironment env,
        IHttpContextAccessor accessor)
    {
        _context = context;
        _logger = logger;
        _env = env;
        _accessor = accessor;
    }

    public async Task<Result<PostResponse>> CreatePostAsync(
          string userId, CreatePostRequest request, CancellationToken ct = default)
    {
        try
        {
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
                return Result.Failure<PostResponse>(UserErrors.UserNotFound);

            var post = new Post
            {
                UserId = userId,
                Content = request.Content,
            };

            if (request.File is not null)
                post.FileUrl = await FileHelper.UploadeFileAsync(request.File, "uploads/posts", _env, _accessor);

            await _context.Posts.AddAsync(post, ct);
            await _context.SaveChangesAsync(ct);

            var response = new PostResponse(
                post.Id,
                post.Content,
                post.FileUrl,
                new UserSummary(userId, author.FullName, author.ProfilePictureUrl),
                IsOwner: true,
                ILiked: false,
                LikesCount: 0,
                CommentsCount: 0,
                post.CreatedAt
            );

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating post for user {UserId}", userId);
            return Result.Failure<PostResponse>(PostErrors.Error);
        }
    }

    public async Task<Result> SoftDeletePostAsync(
        string userId, string postId, CancellationToken ct = default)
    {
        try
        {
            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == postId && p.UserId == userId && !p.IsDeleted, ct);

            if (post is null)
                return Result.Failure(PostErrors.PostNotFound);

            // ✅ SaveChanges الأول — لو فشل الملف مش هيتحذف
            var fileUrl = post.FileUrl;
            post.IsDeleted = true;
            await _context.SaveChangesAsync(ct);

            // ✅ حذف الملف بعد نجاح الـ DB
            if (!string.IsNullOrEmpty(fileUrl))
                FileHelper.DeleteFile(fileUrl, "uploads/posts", _env);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting post {PostId} for user {UserId}", postId, userId);
            return Result.Failure(PostErrors.Error);
        }
    }
    public async Task<Result> UpdatePostAsync(
        string userId, string postId, UpdatePostRequest request, CancellationToken ct = default)
    {
        try
        {
            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == postId && p.UserId == userId && !p.IsDeleted, ct);

            if (post is null)
                return Result.Failure(PostErrors.PostNotFound);

            post.Content = request.Content;
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating post {PostId} for user {UserId}", postId, userId);
            return Result.Failure(PostErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Get Single Post
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<PostResponse>> GetPostAsync(
        string postId, string? currentUserId = null, CancellationToken ct = default)
    {
        try
        {
            var response = await _context.Posts
                .Where(p => p.Id == postId && !p.IsDeleted)
                .Select(p => new PostResponse(
                    p.Id,
                    p.Content,
                    p.FileUrl,
                    new UserSummary(
                        p.UserId,
                        p.User.FullName,
                        p.User.UserProfile == null ? null : p.User.UserProfile.ProfilePictureUrl),
                    IsOwner: p.UserId == currentUserId,
                    ILiked: p.Likes.Any(l => l.UserId == currentUserId),
                    LikesCount: p.Likes.Count,
                    CommentsCount: p.Comments.Count(c => !c.IsDeleted),
                    p.CreatedAt
                ))
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (response is null)
                return Result.Failure<PostResponse>(PostErrors.PostNotFound);

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving post {PostId}", postId);
            return Result.Failure<PostResponse>(PostErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Get Posts by User (profile wall)
    // ════════════════════════════════════════════════════════════════

    public async Task<PaginatedList<PostResponse>> GetPostsByUserAsync(
        string profileUserId, string? currentUserId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var query = _context.Posts
                .Where(p => p.UserId == profileUserId && !p.IsDeleted)
                .AsSplitQuery()
                .ApplyFilters(filters,
                    searchPredicate: x => x.Content != null && x.Content.Contains(filters.SearchValue!),
                    allowedSortColumns: AllowedPostSortColumns)
                .Select(p => new PostResponse(
                    p.Id,
                    p.Content,
                    p.FileUrl,
                    new UserSummary(
                        p.UserId,
                        p.User.FullName,
                        p.User.UserProfile == null ? null : p.User.UserProfile.ProfilePictureUrl),
                    IsOwner: p.UserId == currentUserId,
                    ILiked: p.Likes.Any(l => l.UserId == currentUserId),
                    LikesCount: p.Likes.Count,
                    CommentsCount: p.Comments.Count(c => !c.IsDeleted),
                    p.CreatedAt
                ))
                .AsNoTracking();

            return await query.ToPaginatedListAsync(filters, ct);
        }
        catch (OperationCanceledException)
        {
            // Request was cancelled by the client (HttpContext.RequestAborted).
            // Treat as expected behavior: log at Information and rethrow to let the pipeline handle it.
            _logger.LogInformation("Request cancelled by client while retrieving posts for user {UserId}", profileUserId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving posts for user {UserId}", profileUserId);
            throw;
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Get All Posts (feed)
    // ════════════════════════════════════════════════════════════════

    public async Task<PaginatedList<PostResponse>> GetPostsAsync(
        string? currentUserId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var query = _context.Posts
                .Where(p => !p.IsDeleted)
                .AsSplitQuery()
                .ApplyFilters(filters,
                    searchPredicate: x => x.Content != null && x.Content.Contains(filters.SearchValue!),
                    allowedSortColumns: AllowedPostSortColumns)
                .Select(p => new PostResponse(
                    p.Id,
                    p.Content,
                    p.FileUrl,
                    new UserSummary(
                        p.UserId,
                        p.User.FullName,
                        p.User.UserProfile == null ? null : p.User.UserProfile.ProfilePictureUrl),
                    IsOwner: p.UserId == currentUserId,
                    ILiked: p.Likes.Any(l => l.UserId == currentUserId),
                    LikesCount: p.Likes.Count,
                    CommentsCount: p.Comments.Count(c => !c.IsDeleted),
                    p.CreatedAt
                ))
                .AsNoTracking();

            return await query.ToPaginatedListAsync(filters, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all posts");
            throw;
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Toggle Like
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<ToggleLikeResponse>> ToggleLikeAsync(
        string userId, string postId, CancellationToken ct = default)
    {
        try
        {
            var postExists = await _context.Posts
                .AnyAsync(p => p.Id == postId && !p.IsDeleted, ct);

            if (!postExists)
                return Result.Failure<ToggleLikeResponse>(PostErrors.PostNotFound);

            var existingLike = await _context.PostLikes
                .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId, ct);

            bool isNowLiked;

            if (existingLike is not null)
            {
                _context.PostLikes.Remove(existingLike);
                isNowLiked = false;
            }
            else
            {
                await _context.PostLikes.AddAsync(new PostLike
                {
                    PostId = postId,
                    UserId = userId
                }, ct);
                isNowLiked = true;
            }

            await _context.SaveChangesAsync(ct);

            var likesCount = await _context.PostLikes
                .CountAsync(l => l.PostId == postId, ct);

            return Result.Success(new ToggleLikeResponse(postId, isNowLiked, likesCount));
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            _logger.LogWarning("Duplicate like attempt on post {PostId} by user {UserId}", postId, userId);
            return Result.Failure<ToggleLikeResponse>(PostErrors.AlreadyLiked);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while toggling like on post {PostId} for user {UserId}", postId, userId);
            return Result.Failure<ToggleLikeResponse>(PostErrors.Error);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Get Post Likers
    // ════════════════════════════════════════════════════════════════

    public async Task<Result<PaginatedList<PostLikerResponse>>> GetPostLikersAsync(
        string postId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var postExists = await _context.Posts
                .AnyAsync(p => p.Id == postId && !p.IsDeleted, ct);

            if (!postExists)
                return Result.Failure<PaginatedList<PostLikerResponse>>(PostErrors.PostNotFound);

            var query = _context.PostLikes
                .Where(l => l.PostId == postId)
                .ApplyFilters(filters, allowedSortColumns: AllowedLikerSortColumns)
                .Select(l => new PostLikerResponse(
                    l.UserId,
                    l.User.FullName,
                    l.User.UserProfile == null ? null : l.User.UserProfile.ProfilePictureUrl,
                    l.LikedAt
                ))
                .AsNoTracking();

            var result = await query.ToPaginatedListAsync(filters, ct);
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving likers for post {PostId}", postId);
            return Result.Failure<PaginatedList<PostLikerResponse>>(PostErrors.Error);
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
```

## File: Services/Implementation/ProfileService.cs
```csharp
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
```

## File: Settings/GitHubOAuthOptions.cs
```csharp
namespace Sportiva.Settings;

public class GitHubOAuthOptions
{
    public const string SectionName = "Authentication:GitHub";

    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;
    public List<string> Scopes { get; set; } = [];
}
```

## File: Settings/GoogleOAuthOptions.cs
```csharp
namespace Sportiva.Settings;

public class GoogleOAuthOptions
{
    public const string SectionName = "Authentication:Google";

    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;
    public List<string> Scopes { get; set; } = [];
}
```

## File: Settings/MailSettings.cs
```csharp
namespace Sportiva.Settings;

public class MailSettings
{
    [Required, EmailAddress]
    public string Mail { get; set; } = string.Empty;

    [Required]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Host { get; set; } = string.Empty;

    [Range(100, 999)]
    public int Port { get; set; }
}
```

## File: sportiva-api-reference.html
```html
<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<title>Sportiva — REST API Reference v1</title>
<link rel="preconnect" href="https://fonts.googleapis.com">
<link href="https://fonts.googleapis.com/css2?family=IBM+Plex+Mono:wght@400;500;600&family=Syne:wght@400;600;700;800&family=DM+Sans:ital,wght@0,300;0,400;0,500;1,300&display=swap" rel="stylesheet">
<style>
:root {
  --bg:       #0d0f14;
  --bg2:      #13161d;
  --bg3:      #1a1e28;
  --border:   #252935;
  --border2:  #2e3444;
  --text:     #c8cdd8;
  --text2:    #7a8299;
  --text3:    #444d66;
  --accent:   #4f8ef7;
  --accent2:  #2d5fc4;
  --green:    #3dd68c;
  --amber:    #f7a84f;
  --red:      #f06464;
  --purple:   #a78bfa;
  --teal:     #38ccc0;
  --pink:     #f06fac;
  --GET:      #3dd68c;
  --POST:     #4f8ef7;
  --PUT:      #f7a84f;
  --PATCH:    #f7a84f;
  --DELETE:   #f06464;
  --mono:     'IBM Plex Mono', monospace;
  --display:  'Syne', sans-serif;
  --body:     'DM Sans', sans-serif;
}
*, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }
html { scroll-behavior: smooth; }
body {
  background: var(--bg);
  color: var(--text);
  font-family: var(--body);
  font-size: 14px;
  line-height: 1.6;
  display: flex;
  min-height: 100vh;
}

/* SIDEBAR */
#sidebar {
  width: 230px;
  min-width: 230px;
  background: var(--bg2);
  border-right: 1px solid var(--border);
  position: sticky;
  top: 0;
  height: 100vh;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  padding: 0 0 24px;
  scrollbar-width: thin;
  scrollbar-color: var(--border2) transparent;
}
.sidebar-header {
  padding: 22px 18px 14px;
  border-bottom: 1px solid var(--border);
  position: sticky;
  top: 0;
  background: var(--bg2);
  z-index: 2;
}
.sidebar-logo {
  font-family: var(--display);
  font-size: 17px;
  font-weight: 800;
  color: #fff;
  letter-spacing: -.3px;
}
.sidebar-version {
  font-family: var(--mono);
  font-size: 10px;
  color: var(--accent);
  background: rgba(79,142,247,.1);
  border: 1px solid rgba(79,142,247,.2);
  padding: 1px 6px;
  border-radius: 3px;
  margin-left: 8px;
  vertical-align: middle;
}
.sidebar-base {
  font-family: var(--mono);
  font-size: 10px;
  color: var(--text3);
  margin-top: 6px;
  word-break: break-all;
}
.nav-section {
  padding: 14px 18px 4px;
  font-size: 9.5px;
  font-weight: 600;
  letter-spacing: .1em;
  text-transform: uppercase;
  color: var(--text3);
  font-family: var(--display);
}
.nav-item {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 6px 18px;
  color: var(--text2);
  text-decoration: none;
  font-size: 12.5px;
  font-family: var(--body);
  font-weight: 400;
  border-left: 2px solid transparent;
  transition: all .12s;
}
.nav-item:hover { color: var(--text); background: rgba(255,255,255,.03); }
.nav-item.active { color: #fff; border-left-color: var(--accent); background: rgba(79,142,247,.06); }
.nav-dot {
  width: 6px; height: 6px; border-radius: 50%; flex-shrink: 0;
}

/* MAIN */
#main {
  flex: 1;
  overflow-y: auto;
  padding: 40px 52px 80px;
  max-width: 900px;
}
.page-title {
  font-family: var(--display);
  font-size: 30px;
  font-weight: 800;
  color: #fff;
  letter-spacing: -.5px;
  margin-bottom: 4px;
}
.page-subtitle {
  color: var(--text2);
  font-size: 14px;
  font-weight: 300;
  margin-bottom: 36px;
}

/* SECTION */
.section {
  margin-bottom: 52px;
  scroll-margin-top: 24px;
}
.section-header {
  display: flex;
  align-items: baseline;
  gap: 10px;
  margin-bottom: 16px;
  padding-bottom: 10px;
  border-bottom: 1px solid var(--border);
}
.section-icon {
  width: 8px; height: 8px; border-radius: 50%; flex-shrink: 0; margin-bottom: 1px;
}
.section-title {
  font-family: var(--display);
  font-size: 17px;
  font-weight: 700;
  color: #fff;
  letter-spacing: -.2px;
}
.section-count {
  font-family: var(--mono);
  font-size: 10px;
  color: var(--text3);
  margin-left: auto;
}
.section-note {
  background: var(--bg3);
  border: 1px solid var(--border2);
  border-left: 3px solid var(--accent);
  border-radius: 6px;
  padding: 10px 14px;
  font-size: 12.5px;
  color: var(--text2);
  margin-bottom: 14px;
  line-height: 1.55;
}
.section-note strong { color: var(--text); font-weight: 500; }

/* ENDPOINT */
.endpoint {
  display: grid;
  grid-template-columns: 60px 1fr auto;
  align-items: start;
  gap: 0 10px;
  padding: 9px 14px;
  border-radius: 7px;
  border: 1px solid transparent;
  margin-bottom: 3px;
  transition: background .1s, border-color .1s;
  cursor: default;
}
.endpoint:hover {
  background: var(--bg3);
  border-color: var(--border);
}
.method {
  font-family: var(--mono);
  font-size: 10.5px;
  font-weight: 600;
  padding: 2px 0;
  text-align: right;
  letter-spacing: .03em;
  line-height: 1.9;
}
.m-GET    { color: var(--GET); }
.m-POST   { color: var(--POST); }
.m-PUT    { color: var(--PUT); }
.m-PATCH  { color: var(--PATCH); }
.m-DELETE { color: var(--DELETE); }

.route {
  font-family: var(--mono);
  font-size: 12.5px;
  color: var(--text);
  line-height: 1.9;
  word-break: break-all;
}
.route .seg   { color: var(--text2); }
.route .param { color: var(--purple); }
.route .action{ color: var(--teal); }
.route .me    { color: var(--pink); }

.desc {
  font-size: 11.5px;
  color: var(--text3);
  text-align: right;
  line-height: 1.9;
  white-space: nowrap;
}
.tag {
  display: inline-block;
  font-family: var(--mono);
  font-size: 9px;
  font-weight: 500;
  padding: 1px 5px;
  border-radius: 3px;
  margin-left: 5px;
  vertical-align: middle;
}
.tag-auth   { background: rgba(167,139,250,.12); color: var(--purple); border: 1px solid rgba(167,139,250,.2); }
.tag-admin  { background: rgba(240,100,100,.1);  color: var(--red);    border: 1px solid rgba(240,100,100,.2); }
.tag-paged  { background: rgba(61,214,140,.08);  color: var(--green);  border: 1px solid rgba(61,214,140,.15); }
.tag-search { background: rgba(247,168,79,.08);  color: var(--amber);  border: 1px solid rgba(247,168,79,.15); }

/* GROUP DIVIDER */
.group-label {
  font-size: 10px;
  font-weight: 600;
  letter-spacing: .09em;
  text-transform: uppercase;
  color: var(--text3);
  font-family: var(--display);
  padding: 14px 14px 4px;
}

/* LEGEND */
.legend {
  display: flex;
  gap: 18px;
  flex-wrap: wrap;
  margin-bottom: 32px;
  padding: 14px 18px;
  background: var(--bg2);
  border: 1px solid var(--border);
  border-radius: 8px;
}
.legend-item {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 11.5px;
  color: var(--text2);
}
.legend-method {
  font-family: var(--mono);
  font-size: 10px;
  font-weight: 600;
  min-width: 46px;
}
.base-url-box {
  background: var(--bg3);
  border: 1px solid var(--border2);
  border-radius: 8px;
  padding: 14px 18px;
  margin-bottom: 32px;
  display: flex;
  align-items: center;
  gap: 12px;
}
.base-url-label { font-size: 11px; color: var(--text3); font-family: var(--display); font-weight: 600; letter-spacing: .06em; text-transform: uppercase; }
.base-url-val   { font-family: var(--mono); font-size: 13px; color: #fff; }
</style>
</head>
<body>

<nav id="sidebar">
  <div class="sidebar-header">
    <div>
      <span class="sidebar-logo">Sportiva</span>
      <span class="sidebar-version">v1</span>
    </div>
    <div class="sidebar-base">/api/v1/</div>
  </div>

  <div class="nav-section">Core</div>
  <a class="nav-item active" href="#auth"><span class="nav-dot" style="background:#4f8ef7"></span>Auth</a>
  <a class="nav-item" href="#users"><span class="nav-dot" style="background:#a78bfa"></span>Users &amp; Profiles</a>
  <a class="nav-item" href="#followers"><span class="nav-dot" style="background:#f06fac"></span>Followers</a>

  <div class="nav-section">Social</div>
  <a class="nav-item" href="#posts"><span class="nav-dot" style="background:#3dd68c"></span>Posts</a>
  <a class="nav-item" href="#comments"><span class="nav-dot" style="background:#3dd68c"></span>Comments &amp; Replies</a>
  <a class="nav-item" href="#likes"><span class="nav-dot" style="background:#3dd68c"></span>Likes</a>

  <div class="nav-section">Venues</div>
  <a class="nav-item" href="#clubs"><span class="nav-dot" style="background:#f7a84f"></span>Clubs</a>
  <a class="nav-item" href="#subscriptions"><span class="nav-dot" style="background:#f7a84f"></span>Subscriptions</a>
  <a class="nav-item" href="#courts"><span class="nav-dot" style="background:#f7a84f"></span>Courts</a>
  <a class="nav-item" href="#timeslots"><span class="nav-dot" style="background:#f7a84f"></span>Time Slots</a>
  <a class="nav-item" href="#bookings"><span class="nav-dot" style="background:#f7a84f"></span>Bookings</a>
  <a class="nav-item" href="#reviews"><span class="nav-dot" style="background:#f7a84f"></span>Reviews</a>

  <div class="nav-section">Sport</div>
  <a class="nav-item" href="#matches"><span class="nav-dot" style="background:#38ccc0"></span>Friendly Matches</a>
  <a class="nav-item" href="#join-requests"><span class="nav-dot" style="background:#38ccc0"></span>Join Requests</a>
  <a class="nav-item" href="#tournaments"><span class="nav-dot" style="background:#38ccc0"></span>Tournaments</a>
  <a class="nav-item" href="#tournament-matches"><span class="nav-dot" style="background:#38ccc0"></span>Tournament Matches</a>

  <div class="nav-section">Comms</div>
  <a class="nav-item" href="#messaging"><span class="nav-dot" style="background:#f06464"></span>Messaging</a>
  <a class="nav-item" href="#notifications"><span class="nav-dot" style="background:#f06464"></span>Notifications</a>

  <div class="nav-section">Platform</div>
  <a class="nav-item" href="#memberships"><span class="nav-dot" style="background:#7a8299"></span>Memberships</a>
  <a class="nav-item" href="#admin"><span class="nav-dot" style="background:#f06464"></span>Admin</a>
</nav>

<main id="main">
  <h1 class="page-title">REST API Reference</h1>
  <p class="page-subtitle">Sportiva Platform — Complete endpoint architecture · 16 modules · Clean RESTful design</p>

  <div class="base-url-box">
    <span class="base-url-label">Base URL</span>
    <span class="base-url-val">https://api.sportiva.com/api/v1</span>
  </div>

  <div class="legend">
    <div class="legend-item"><span class="legend-method m-GET">GET</span> Retrieve resource(s)</div>
    <div class="legend-item"><span class="legend-method m-POST">POST</span> Create / action</div>
    <div class="legend-item"><span class="legend-method m-PUT">PUT</span> Replace resource</div>
    <div class="legend-item"><span class="legend-method m-PATCH">PATCH</span> Partial update</div>
    <div class="legend-item"><span class="legend-method m-DELETE">DELETE</span> Remove resource</div>
    <div class="legend-item"><span class="tag tag-auth">AUTH</span> Requires JWT</div>
    <div class="legend-item"><span class="tag tag-admin">ADMIN</span> Admin role only</div>
    <div class="legend-item"><span class="tag tag-paged">PAGED</span> Returns PagedResponse&lt;T&gt;</div>
    <div class="legend-item"><span class="tag tag-search">?q=</span> Supports query params</div>
  </div>

  <!-- AUTH -->
  <div class="section" id="auth">
    <div class="section-header">
      <span class="section-icon" style="background:#4f8ef7"></span>
      <span class="section-title">Auth</span>
      <span class="section-count">6 endpoints</span>
    </div>
    <div class="section-note">
      Stateless JWT auth. <strong>POST /register</strong> and <strong>POST /login</strong> return <code>AuthResponse</code> with a short-lived access token and a long-lived refresh token. Refresh tokens are rotated on every use. Email confirmation is a fire-and-forget POST. Password reset is a two-step flow: request a code, then exchange code + new password.
    </div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/auth/</span>register</span><span class="desc">Create account</span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/auth/</span>login</span><span class="desc">Get tokens</span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/auth/</span>refresh</span><span class="desc">Rotate refresh token</span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/auth/</span>logout</span><span class="desc">Revoke refresh token<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/auth/</span>forgot-password</span><span class="desc">Send reset code</span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/auth/</span>reset-password</span><span class="desc">Exchange code + set password</span></div>
  </div>

  <!-- USERS -->
  <div class="section" id="users">
    <div class="section-header">
      <span class="section-icon" style="background:#a78bfa"></span>
      <span class="section-title">Users &amp; Profiles</span>
      <span class="section-count">7 endpoints</span>
    </div>
    <div class="section-note">
      <strong>/me</strong> always refers to the authenticated caller. <strong>GET /users/{userId}</strong> is the public profile view. Search lives on <strong>/users</strong> with <code>?q=</code>. Avatar upload is a PATCH to keep the profile PATCH partial and clean.
    </div>
    <div class="group-label">Current user</div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/</span><span class="me">me</span></span><span class="desc">My profile<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-PATCH">PATCH</span><span class="route"><span class="seg">/</span><span class="me">me</span></span><span class="desc">Update my profile<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-PATCH">PATCH</span><span class="route"><span class="seg">/</span><span class="me">me</span><span class="seg">/</span>avatar</span><span class="desc">Upload profile picture<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-PATCH">PATCH</span><span class="route"><span class="seg">/</span><span class="me">me</span><span class="seg">/</span>cover</span><span class="desc">Upload cover image<span class="tag tag-auth">AUTH</span></span></div>
    <div class="group-label">Public user lookup</div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/users</span></span><span class="desc">Search users<span class="tag tag-auth">AUTH</span><span class="tag tag-paged">PAGED</span><span class="tag tag-search">?q=</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/users/</span><span class="param">{userId}</span></span><span class="desc">Public profile</span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/users/</span><span class="param">{userId}</span><span class="seg">/posts</span></span><span class="desc">User's posts<span class="tag tag-paged">PAGED</span></span></div>
  </div>

  <!-- FOLLOWERS -->
  <div class="section" id="followers">
    <div class="section-header">
      <span class="section-icon" style="background:#f06fac"></span>
      <span class="section-title">Followers</span>
      <span class="section-count">6 endpoints</span>
    </div>
    <div class="section-note">
      Follow/unfollow is a <strong>POST/DELETE on the relationship resource</strong>: <code>/users/{userId}/follow</code>. Each user only accesses their own lists via <strong>/me/followers</strong> and <strong>/me/following</strong>.
    </div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/users/</span><span class="param">{userId}</span><span class="seg">/</span><span class="action">follow</span></span><span class="desc">Follow a user<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-DELETE">DELETE</span><span class="route"><span class="seg">/users/</span><span class="param">{userId}</span><span class="seg">/</span><span class="action">follow</span></span><span class="desc">Unfollow a user<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/users/</span><span class="param">{userId}</span><span class="seg">/followers</span></span><span class="desc">User's followers<span class="tag tag-paged">PAGED</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/users/</span><span class="param">{userId}</span><span class="seg">/following</span></span><span class="desc">Accounts user follows<span class="tag tag-paged">PAGED</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/</span><span class="me">me</span><span class="seg">/followers</span></span><span class="desc">My followers<span class="tag tag-auth">AUTH</span><span class="tag tag-paged">PAGED</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/</span><span class="me">me</span><span class="seg">/following</span></span><span class="desc">Accounts I follow<span class="tag tag-auth">AUTH</span><span class="tag tag-paged">PAGED</span></span></div>
  </div>

  <!-- POSTS -->
  <div class="section" id="posts">
    <div class="section-header">
      <span class="section-icon" style="background:#3dd68c"></span>
      <span class="section-title">Posts</span>
      <span class="section-count">6 endpoints</span>
    </div>
    <div class="section-note">
      <strong>GET /posts</strong> returns the authenticated user's social feed. Feed supports cursor-based pagination via <code>?before=</code> for infinite scroll.
    </div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/posts</span></span><span class="desc">Social feed<span class="tag tag-auth">AUTH</span><span class="tag tag-paged">PAGED</span></span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/posts</span></span><span class="desc">Create post<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/posts/</span><span class="param">{postId}</span></span><span class="desc">Get single post</span></div>
    <div class="endpoint"><span class="method m-PATCH">PATCH</span><span class="route"><span class="seg">/posts/</span><span class="param">{postId}</span></span><span class="desc">Edit post<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-DELETE">DELETE</span><span class="route"><span class="seg">/posts/</span><span class="param">{postId}</span></span><span class="desc">Delete post<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/</span><span class="me">me</span><span class="seg">/posts</span></span><span class="desc">My posts<span class="tag tag-auth">AUTH</span><span class="tag tag-paged">PAGED</span></span></div>
  </div>

  <!-- COMMENTS -->
  <div class="section" id="comments">
    <div class="section-header">
      <span class="section-icon" style="background:#3dd68c"></span>
      <span class="section-title">Comments &amp; Replies</span>
      <span class="section-count">10 endpoints</span>
    </div>
    <div class="section-note">
      Comments are a true child resource of posts. Replies are nested under comments. <strong>Max one level of nesting</strong> — replies-of-replies go to the same comment (flat threading).
    </div>
    <div class="group-label">Comments</div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/posts/</span><span class="param">{postId}</span><span class="seg">/comments</span></span><span class="desc">List comments<span class="tag tag-paged">PAGED</span></span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/posts/</span><span class="param">{postId}</span><span class="seg">/comments</span></span><span class="desc">Add comment<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-PATCH">PATCH</span><span class="route"><span class="seg">/posts/</span><span class="param">{postId}</span><span class="seg">/comments/</span><span class="param">{commentId}</span></span><span class="desc">Edit comment<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-DELETE">DELETE</span><span class="route"><span class="seg">/posts/</span><span class="param">{postId}</span><span class="seg">/comments/</span><span class="param">{commentId}</span></span><span class="desc">Delete comment<span class="tag tag-auth">AUTH</span></span></div>
    <div class="group-label">Replies</div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/posts/</span><span class="param">{postId}</span><span class="seg">/comments/</span><span class="param">{commentId}</span><span class="seg">/replies</span></span><span class="desc">List replies<span class="tag tag-paged">PAGED</span></span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/posts/</span><span class="param">{postId}</span><span class="seg">/comments/</span><span class="param">{commentId}</span><span class="seg">/replies</span></span><span class="desc">Add reply<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-PATCH">PATCH</span><span class="route"><span class="seg">/posts/</span><span class="param">{postId}</span><span class="seg">/comments/</span><span class="param">{commentId}</span><span class="seg">/replies/</span><span class="param">{replyId}</span></span><span class="desc">Edit reply<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-DELETE">DELETE</span><span class="route"><span class="seg">/posts/</span><span class="param">{postId}</span><span class="seg">/comments/</span><span class="param">{commentId}</span><span class="seg">/replies/</span><span class="param">{replyId}</span></span><span class="desc">Delete reply<span class="tag tag-auth">AUTH</span></span></div>
    <div class="group-label">Comment &amp; reply likes</div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/posts/</span><span class="param">{postId}</span><span class="seg">/comments/</span><span class="param">{commentId}</span><span class="seg">/</span><span class="action">like</span></span><span class="desc">Like comment<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-DELETE">DELETE</span><span class="route"><span class="seg">/posts/</span><span class="param">{postId}</span><span class="seg">/comments/</span><span class="param">{commentId}</span><span class="seg">/</span><span class="action">like</span></span><span class="desc">Unlike comment<span class="tag tag-auth">AUTH</span></span></div>
  </div>

  <!-- LIKES -->
  <div class="section" id="likes">
    <div class="section-header">
      <span class="section-icon" style="background:#3dd68c"></span>
      <span class="section-title">Post Likes</span>
      <span class="section-count">3 endpoints</span>
    </div>
    <div class="section-note">
      Like/unlike are POST/DELETE on <strong>/posts/{postId}/like</strong> — treating the like as a resource on the post, consistent with the follow/unfollow pattern.
    </div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/posts/</span><span class="param">{postId}</span><span class="seg">/</span><span class="action">like</span></span><span class="desc">Like post<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-DELETE">DELETE</span><span class="route"><span class="seg">/posts/</span><span class="param">{postId}</span><span class="seg">/</span><span class="action">like</span></span><span class="desc">Unlike post<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/posts/</span><span class="param">{postId}</span><span class="seg">/likes</span></span><span class="desc">Who liked this<span class="tag tag-paged">PAGED</span></span></div>
  </div>

  <!-- CLUBS -->
  <div class="section" id="clubs">
    <div class="section-header">
      <span class="section-icon" style="background:#f7a84f"></span>
      <span class="section-title">Clubs</span>
      <span class="section-count">7 endpoints</span>
    </div>
    <div class="section-note">
      A club is a top-level resource — independently searchable and bookable. <strong>/me/clubs</strong> returns clubs owned by the authenticated user. Status toggle uses a dedicated <strong>PATCH .../status</strong> action.
    </div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/clubs</span></span><span class="desc">Browse clubs<span class="tag tag-paged">PAGED</span><span class="tag tag-search">?city= ?sport=</span></span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/clubs</span></span><span class="desc">Create club<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/clubs/</span><span class="param">{clubId}</span></span><span class="desc">Club detail</span></div>
    <div class="endpoint"><span class="method m-PATCH">PATCH</span><span class="route"><span class="seg">/clubs/</span><span class="param">{clubId}</span></span><span class="desc">Update club<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-DELETE">DELETE</span><span class="route"><span class="seg">/clubs/</span><span class="param">{clubId}</span></span><span class="desc">Delete club<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-PATCH">PATCH</span><span class="route"><span class="seg">/clubs/</span><span class="param">{clubId}</span><span class="seg">/</span><span class="action">status</span></span><span class="desc">Toggle active/inactive<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/</span><span class="me">me</span><span class="seg">/clubs</span></span><span class="desc">My clubs<span class="tag tag-auth">AUTH</span><span class="tag tag-paged">PAGED</span></span></div>
  </div>

  <!-- SUBSCRIPTIONS -->
  <div class="section" id="subscriptions">
    <div class="section-header">
      <span class="section-icon" style="background:#f7a84f"></span>
      <span class="section-title">Subscription Plans &amp; Club Subscriptions</span>
      <span class="section-count">10 endpoints</span>
    </div>
    <div class="section-note">
      <strong>Plans</strong> are platform-level resources managed by admins. <strong>Club subscriptions</strong> are nested under clubs. The <strong>renew</strong> action POSTs a new subscription to keep history intact.
    </div>
    <div class="group-label">Plans (admin-managed)</div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/subscription-plans</span></span><span class="desc">List all plans</span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/subscription-plans</span></span><span class="desc">Create plan<span class="tag tag-admin">ADMIN</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/subscription-plans/</span><span class="param">{planId}</span></span><span class="desc">Plan detail</span></div>
    <div class="endpoint"><span class="method m-PATCH">PATCH</span><span class="route"><span class="seg">/subscription-plans/</span><span class="param">{planId}</span></span><span class="desc">Update plan<span class="tag tag-admin">ADMIN</span></span></div>
    <div class="endpoint"><span class="method m-DELETE">DELETE</span><span class="route"><span class="seg">/subscription-plans/</span><span class="param">{planId}</span></span><span class="desc">Archive plan<span class="tag tag-admin">ADMIN</span></span></div>
    <div class="group-label">Club subscriptions</div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/clubs/</span><span class="param">{clubId}</span><span class="seg">/subscription</span></span><span class="desc">Active subscription<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/clubs/</span><span class="param">{clubId}</span><span class="seg">/subscriptions</span></span><span class="desc">Subscription history<span class="tag tag-auth">AUTH</span><span class="tag tag-paged">PAGED</span></span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/clubs/</span><span class="param">{clubId}</span><span class="seg">/subscriptions</span></span><span class="desc">Subscribe to plan<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/clubs/</span><span class="param">{clubId}</span><span class="seg">/subscriptions/</span><span class="action">renew</span></span><span class="desc">Renew subscription<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/clubs/</span><span class="param">{clubId}</span><span class="seg">/subscriptions/</span><span class="action">cancel</span></span><span class="desc">Cancel subscription<span class="tag tag-auth">AUTH</span></span></div>
  </div>

  <!-- COURTS -->
  <div class="section" id="courts">
    <div class="section-header">
      <span class="section-icon" style="background:#f7a84f"></span>
      <span class="section-title">Courts</span>
      <span class="section-count">8 endpoints</span>
    </div>
    <div class="section-note">
      Courts are nested under clubs. <strong>GET /courts</strong> also exists as a top-level search for the "find a court near me" discovery flow with location/sport filters.
    </div>
    <div class="group-label">Discovery (flat)</div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/courts</span></span><span class="desc">Search all courts<span class="tag tag-paged">PAGED</span><span class="tag tag-search">?sport= ?city= ?date=</span></span></div>
    <div class="group-label">Club-scoped</div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/clubs/</span><span class="param">{clubId}</span><span class="seg">/courts</span></span><span class="desc">Club's courts<span class="tag tag-paged">PAGED</span></span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/clubs/</span><span class="param">{clubId}</span><span class="seg">/courts</span></span><span class="desc">Add court<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/clubs/</span><span class="param">{clubId}</span><span class="seg">/courts/</span><span class="param">{courtId}</span></span><span class="desc">Court detail</span></div>
    <div class="endpoint"><span class="method m-PATCH">PATCH</span><span class="route"><span class="seg">/clubs/</span><span class="param">{clubId}</span><span class="seg">/courts/</span><span class="param">{courtId}</span></span><span class="desc">Update court<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-DELETE">DELETE</span><span class="route"><span class="seg">/clubs/</span><span class="param">{clubId}</span><span class="seg">/courts/</span><span class="param">{courtId}</span></span><span class="desc">Remove court<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-PATCH">PATCH</span><span class="route"><span class="seg">/clubs/</span><span class="param">{clubId}</span><span class="seg">/courts/</span><span class="param">{courtId}</span><span class="seg">/</span><span class="action">status</span></span><span class="desc">Toggle active<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/clubs/</span><span class="param">{clubId}</span><span class="seg">/courts/</span><span class="param">{courtId}</span><span class="seg">/availability</span></span><span class="desc">Open slots<span class="tag tag-search">?date=</span></span></div>
  </div>

  <!-- TIME SLOTS -->
  <div class="section" id="timeslots">
    <div class="section-header">
      <span class="section-icon" style="background:#f7a84f"></span>
      <span class="section-title">Time Slots</span>
      <span class="section-count">6 endpoints</span>
    </div>
    <div class="section-note">
      Time slots are nested under courts. <code>POST /bulk</code> lets court managers schedule entire days or weeks at once — avoids N round-trips from the management dashboard.
    </div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/courts/</span><span class="param">{courtId}</span><span class="seg">/time-slots</span></span><span class="desc">List slots<span class="tag tag-search">?date= ?available=</span></span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/courts/</span><span class="param">{courtId}</span><span class="seg">/time-slots</span></span><span class="desc">Create slot<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/courts/</span><span class="param">{courtId}</span><span class="seg">/time-slots/</span><span class="action">bulk</span></span><span class="desc">Bulk create slots<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/courts/</span><span class="param">{courtId}</span><span class="seg">/time-slots/</span><span class="param">{slotId}</span></span><span class="desc">Slot detail</span></div>
    <div class="endpoint"><span class="method m-PATCH">PATCH</span><span class="route"><span class="seg">/courts/</span><span class="param">{courtId}</span><span class="seg">/time-slots/</span><span class="param">{slotId}</span></span><span class="desc">Update slot<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-DELETE">DELETE</span><span class="route"><span class="seg">/courts/</span><span class="param">{courtId}</span><span class="seg">/time-slots/</span><span class="param">{slotId}</span></span><span class="desc">Delete slot<span class="tag tag-auth">AUTH</span></span></div>
  </div>

  <!-- BOOKINGS -->
  <div class="section" id="bookings">
    <div class="section-header">
      <span class="section-icon" style="background:#f7a84f"></span>
      <span class="section-title">Bookings</span>
      <span class="section-count">7 endpoints</span>
    </div>
    <div class="section-note">
      Bookings are a <strong>top-level resource</strong> — not nested under courts. <strong>/me/bookings</strong> is the primary mobile screen. Cancellation is a POST action (not DELETE) because the booking record is retained for history and review eligibility.
    </div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/</span><span class="me">me</span><span class="seg">/bookings</span></span><span class="desc">My bookings<span class="tag tag-auth">AUTH</span><span class="tag tag-paged">PAGED</span><span class="tag tag-search">?status=</span></span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/bookings</span></span><span class="desc">Create booking<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/bookings/</span><span class="param">{bookingId}</span></span><span class="desc">Booking detail<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/bookings/</span><span class="param">{bookingId}</span><span class="seg">/</span><span class="action">cancel</span></span><span class="desc">Cancel booking<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/courts/</span><span class="param">{courtId}</span><span class="seg">/bookings</span></span><span class="desc">Court's bookings<span class="tag tag-auth">AUTH</span><span class="tag tag-paged">PAGED</span><span class="tag tag-search">?date=</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/clubs/</span><span class="param">{clubId}</span><span class="seg">/bookings</span></span><span class="desc">Club's bookings<span class="tag tag-auth">AUTH</span><span class="tag tag-paged">PAGED</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/bookings/</span><span class="param">{bookingId}</span><span class="seg">/receipt</span></span><span class="desc">Booking receipt<span class="tag tag-auth">AUTH</span></span></div>
  </div>

  <!-- REVIEWS -->
  <div class="section" id="reviews">
    <div class="section-header">
      <span class="section-icon" style="background:#f7a84f"></span>
      <span class="section-title">Reviews</span>
      <span class="section-count">8 endpoints</span>
    </div>
    <div class="section-note">
      Reviews are created via a booking ID — enforces <strong>one booking → one review</strong> server-side. Reading reviews is by court or by club. <strong>/me/reviews</strong> supports the user's history screen.
    </div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/reviews</span></span><span class="desc">Submit review (via bookingId)<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/reviews/</span><span class="param">{reviewId}</span></span><span class="desc">Review detail</span></div>
    <div class="endpoint"><span class="method m-PATCH">PATCH</span><span class="route"><span class="seg">/reviews/</span><span class="param">{reviewId}</span></span><span class="desc">Edit review<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-DELETE">DELETE</span><span class="route"><span class="seg">/reviews/</span><span class="param">{reviewId}</span></span><span class="desc">Delete review<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/courts/</span><span class="param">{courtId}</span><span class="seg">/reviews</span></span><span class="desc">Court reviews<span class="tag tag-paged">PAGED</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/clubs/</span><span class="param">{clubId}</span><span class="seg">/reviews</span></span><span class="desc">Club reviews<span class="tag tag-paged">PAGED</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/</span><span class="me">me</span><span class="seg">/reviews</span></span><span class="desc">My reviews<span class="tag tag-auth">AUTH</span><span class="tag tag-paged">PAGED</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/bookings/</span><span class="param">{bookingId}</span><span class="seg">/review</span></span><span class="desc">Review for booking<span class="tag tag-auth">AUTH</span></span></div>
  </div>

  <!-- FRIENDLY MATCHES -->
  <div class="section" id="matches">
    <div class="section-header">
      <span class="section-icon" style="background:#38ccc0"></span>
      <span class="section-title">Friendly Matches</span>
      <span class="section-count">9 endpoints</span>
    </div>
    <div class="section-note">
      Friendly matches are <strong>top-level</strong> — discovery feed spans all venues. The <strong>leave</strong> action is a POST because it may trigger downstream effects like promoting a waitlist member.
    </div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/friendly-matches</span></span><span class="desc">Browse open matches<span class="tag tag-paged">PAGED</span><span class="tag tag-search">?sport= ?date= ?city=</span></span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/friendly-matches</span></span><span class="desc">Create match<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/friendly-matches/</span><span class="param">{matchId}</span></span><span class="desc">Match detail</span></div>
    <div class="endpoint"><span class="method m-PATCH">PATCH</span><span class="route"><span class="seg">/friendly-matches/</span><span class="param">{matchId}</span></span><span class="desc">Update match<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-DELETE">DELETE</span><span class="route"><span class="seg">/friendly-matches/</span><span class="param">{matchId}</span></span><span class="desc">Cancel match<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/friendly-matches/</span><span class="param">{matchId}</span><span class="seg">/participants</span></span><span class="desc">Participant list</span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/friendly-matches/</span><span class="param">{matchId}</span><span class="seg">/</span><span class="action">leave</span></span><span class="desc">Leave match<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/</span><span class="me">me</span><span class="seg">/friendly-matches</span></span><span class="desc">My matches<span class="tag tag-auth">AUTH</span><span class="tag tag-paged">PAGED</span><span class="tag tag-search">?role=organizer|participant</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/courts/</span><span class="param">{courtId}</span><span class="seg">/friendly-matches</span></span><span class="desc">Matches at court<span class="tag tag-paged">PAGED</span></span></div>
  </div>

  <!-- JOIN REQUESTS -->
  <div class="section" id="join-requests">
    <div class="section-header">
      <span class="section-icon" style="background:#38ccc0"></span>
      <span class="section-title">Match Join Requests</span>
      <span class="section-count">7 endpoints</span>
    </div>
    <div class="section-note">
      <strong>Accept</strong> and <strong>reject</strong> are separate POST actions rather than a status PATCH — they have distinct side effects (slot decrement, notifications) and the explicit verb is safer to authorise.
    </div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/friendly-matches/</span><span class="param">{matchId}</span><span class="seg">/</span><span class="action">join</span></span><span class="desc">Request to join<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/friendly-matches/</span><span class="param">{matchId}</span><span class="seg">/join-requests</span></span><span class="desc">Pending requests<span class="tag tag-auth">AUTH</span><span class="tag tag-paged">PAGED</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/friendly-matches/</span><span class="param">{matchId}</span><span class="seg">/join-requests/</span><span class="param">{requestId}</span></span><span class="desc">Request detail<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/friendly-matches/</span><span class="param">{matchId}</span><span class="seg">/join-requests/</span><span class="param">{requestId}</span><span class="seg">/</span><span class="action">accept</span></span><span class="desc">Accept player<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/friendly-matches/</span><span class="param">{matchId}</span><span class="seg">/join-requests/</span><span class="param">{requestId}</span><span class="seg">/</span><span class="action">reject</span></span><span class="desc">Reject player<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-DELETE">DELETE</span><span class="route"><span class="seg">/friendly-matches/</span><span class="param">{matchId}</span><span class="seg">/join-requests/</span><span class="param">{requestId}</span></span><span class="desc">Withdraw request<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/</span><span class="me">me</span><span class="seg">/join-requests</span></span><span class="desc">My join requests<span class="tag tag-auth">AUTH</span><span class="tag tag-paged">PAGED</span><span class="tag tag-search">?status=</span></span></div>
  </div>

  <!-- TOURNAMENTS -->
  <div class="section" id="tournaments">
    <div class="section-header">
      <span class="section-icon" style="background:#38ccc0"></span>
      <span class="section-title">Tournaments</span>
      <span class="section-count">9 endpoints</span>
    </div>
    <div class="section-note">
      Tournaments are top-level — cross-venue discovery. <strong>JOIN and LEAVE</strong> are POST actions. Stats come from the tournament detail response to avoid an extra round-trip.
    </div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/tournaments</span></span><span class="desc">Browse tournaments<span class="tag tag-paged">PAGED</span><span class="tag tag-search">?sport= ?status= ?city=</span></span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/tournaments</span></span><span class="desc">Create tournament<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/tournaments/</span><span class="param">{tournamentId}</span></span><span class="desc">Tournament detail</span></div>
    <div class="endpoint"><span class="method m-PATCH">PATCH</span><span class="route"><span class="seg">/tournaments/</span><span class="param">{tournamentId}</span></span><span class="desc">Update tournament<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-DELETE">DELETE</span><span class="route"><span class="seg">/tournaments/</span><span class="param">{tournamentId}</span></span><span class="desc">Cancel tournament<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/tournaments/</span><span class="param">{tournamentId}</span><span class="seg">/</span><span class="action">join</span></span><span class="desc">Register for tournament<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/tournaments/</span><span class="param">{tournamentId}</span><span class="seg">/</span><span class="action">leave</span></span><span class="desc">Withdraw from tournament<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/tournaments/</span><span class="param">{tournamentId}</span><span class="seg">/participants</span></span><span class="desc">Participant list<span class="tag tag-paged">PAGED</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/</span><span class="me">me</span><span class="seg">/tournaments</span></span><span class="desc">My tournaments<span class="tag tag-auth">AUTH</span><span class="tag tag-paged">PAGED</span></span></div>
  </div>

  <!-- TOURNAMENT MATCHES -->
  <div class="section" id="tournament-matches">
    <div class="section-header">
      <span class="section-icon" style="background:#38ccc0"></span>
      <span class="section-title">Tournament Matches</span>
      <span class="section-count">6 endpoints</span>
    </div>
    <div class="section-note">
      Always nested under their tournament. <strong>set-winner</strong> is an explicit POST action — it triggers bracket progression logic that should be distinct from a plain field update.
    </div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/tournaments/</span><span class="param">{tournamentId}</span><span class="seg">/matches</span></span><span class="desc">Bracket / match list<span class="tag tag-search">?round=</span></span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/tournaments/</span><span class="param">{tournamentId}</span><span class="seg">/matches</span></span><span class="desc">Schedule match<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/tournaments/</span><span class="param">{tournamentId}</span><span class="seg">/matches/</span><span class="param">{matchId}</span></span><span class="desc">Match detail</span></div>
    <div class="endpoint"><span class="method m-PATCH">PATCH</span><span class="route"><span class="seg">/tournaments/</span><span class="param">{tournamentId}</span><span class="seg">/matches/</span><span class="param">{matchId}</span></span><span class="desc">Update match<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-DELETE">DELETE</span><span class="route"><span class="seg">/tournaments/</span><span class="param">{tournamentId}</span><span class="seg">/matches/</span><span class="param">{matchId}</span></span><span class="desc">Remove match<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/tournaments/</span><span class="param">{tournamentId}</span><span class="seg">/matches/</span><span class="param">{matchId}</span><span class="seg">/</span><span class="action">set-winner</span></span><span class="desc">Record result<span class="tag tag-auth">AUTH</span></span></div>
  </div>

  <!-- MESSAGING -->
  <div class="section" id="messaging">
    <div class="section-header">
      <span class="section-icon" style="background:#f06464"></span>
      <span class="section-title">Messaging &amp; Conversations</span>
      <span class="section-count">8 endpoints</span>
    </div>
    <div class="section-note">
      Conversations are identified by the <strong>other party's userId</strong> — no separate creation step. Sending to a new user auto-creates the conversation. <strong>mark-read</strong> batch-marks all messages as read in one call.
    </div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/</span><span class="me">me</span><span class="seg">/conversations</span></span><span class="desc">Inbox<span class="tag tag-auth">AUTH</span><span class="tag tag-paged">PAGED</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/conversations/</span><span class="param">{userId}</span></span><span class="desc">Conversation with user<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/conversations/</span><span class="param">{userId}</span><span class="seg">/messages</span></span><span class="desc">Message history<span class="tag tag-auth">AUTH</span><span class="tag tag-paged">PAGED</span><span class="tag tag-search">?before=</span></span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/conversations/</span><span class="param">{userId}</span><span class="seg">/messages</span></span><span class="desc">Send message<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/conversations/</span><span class="param">{userId}</span><span class="seg">/messages/</span><span class="param">{messageId}</span></span><span class="desc">Single message<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-DELETE">DELETE</span><span class="route"><span class="seg">/conversations/</span><span class="param">{userId}</span><span class="seg">/messages/</span><span class="param">{messageId}</span></span><span class="desc">Delete message<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/conversations/</span><span class="param">{userId}</span><span class="seg">/</span><span class="action">mark-read</span></span><span class="desc">Mark all as read<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-DELETE">DELETE</span><span class="route"><span class="seg">/conversations/</span><span class="param">{userId}</span></span><span class="desc">Archive conversation<span class="tag tag-auth">AUTH</span></span></div>
  </div>

  <!-- NOTIFICATIONS -->
  <div class="section" id="notifications">
    <div class="section-header">
      <span class="section-icon" style="background:#f06464"></span>
      <span class="section-title">Notifications</span>
      <span class="section-count">8 endpoints</span>
    </div>
    <div class="section-note">
      All notification endpoints live under <strong>/me/</strong>. The <strong>counters</strong> endpoint is a lightweight poll for the bell badge. Preferences use a <strong>bulk PUT</strong> — client sends the full preference matrix at once.
    </div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/</span><span class="me">me</span><span class="seg">/notifications</span></span><span class="desc">Notification list<span class="tag tag-auth">AUTH</span><span class="tag tag-paged">PAGED</span><span class="tag tag-search">?unread=true</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/</span><span class="me">me</span><span class="seg">/notifications/counters</span></span><span class="desc">Unread badge count<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/</span><span class="me">me</span><span class="seg">/notifications/</span><span class="param">{notificationId}</span><span class="seg">/</span><span class="action">read</span></span><span class="desc">Mark one as read<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/</span><span class="me">me</span><span class="seg">/notifications/</span><span class="action">mark-all-read</span></span><span class="desc">Mark all as read<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-DELETE">DELETE</span><span class="route"><span class="seg">/</span><span class="me">me</span><span class="seg">/notifications/</span><span class="param">{notificationId}</span></span><span class="desc">Delete notification<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-DELETE">DELETE</span><span class="route"><span class="seg">/</span><span class="me">me</span><span class="seg">/notifications</span></span><span class="desc">Clear all<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/</span><span class="me">me</span><span class="seg">/notification-preferences</span></span><span class="desc">Preferences<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-PUT">PUT</span><span class="route"><span class="seg">/</span><span class="me">me</span><span class="seg">/notification-preferences</span></span><span class="desc">Bulk update preferences<span class="tag tag-auth">AUTH</span></span></div>
  </div>

  <!-- MEMBERSHIPS -->
  <div class="section" id="memberships">
    <div class="section-header">
      <span class="section-icon" style="background:#7a8299"></span>
      <span class="section-title">Membership Upgrade Requests</span>
      <span class="section-count">7 endpoints</span>
    </div>
    <div class="section-note">
      Regular users interact via <strong>/me/membership-request</strong> (singular — one active request at a time). Admins use <strong>/membership-requests</strong> to manage the queue. Approve and reject carry distinct side effects.
    </div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/</span><span class="me">me</span><span class="seg">/membership-request</span></span><span class="desc">My current request<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/membership-requests</span></span><span class="desc">Submit request<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-DELETE">DELETE</span><span class="route"><span class="seg">/membership-requests/</span><span class="param">{requestId}</span></span><span class="desc">Withdraw request<span class="tag tag-auth">AUTH</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/membership-requests</span></span><span class="desc">All requests<span class="tag tag-admin">ADMIN</span><span class="tag tag-paged">PAGED</span><span class="tag tag-search">?status=</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/membership-requests/</span><span class="param">{requestId}</span></span><span class="desc">Request detail<span class="tag tag-admin">ADMIN</span></span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/membership-requests/</span><span class="param">{requestId}</span><span class="seg">/</span><span class="action">approve</span></span><span class="desc">Approve request<span class="tag tag-admin">ADMIN</span></span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/membership-requests/</span><span class="param">{requestId}</span><span class="seg">/</span><span class="action">reject</span></span><span class="desc">Reject with note<span class="tag tag-admin">ADMIN</span></span></div>
  </div>

  <!-- ADMIN -->
  <div class="section" id="admin">
    <div class="section-header">
      <span class="section-icon" style="background:#f06464"></span>
      <span class="section-title">Admin</span>
      <span class="section-count">12 endpoints</span>
    </div>
    <div class="section-note">
      All admin endpoints are prefixed with <strong>/admin/</strong> — a dedicated sub-path that can be protected at the gateway level (IP allowlist, separate auth middleware) independently of the public API.
    </div>
    <div class="group-label">User management</div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/admin/users</span></span><span class="desc">All users<span class="tag tag-admin">ADMIN</span><span class="tag tag-paged">PAGED</span><span class="tag tag-search">?q= ?role=</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/admin/users/</span><span class="param">{userId}</span></span><span class="desc">User detail<span class="tag tag-admin">ADMIN</span></span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/admin/users/</span><span class="param">{userId}</span><span class="seg">/</span><span class="action">ban</span></span><span class="desc">Ban user<span class="tag tag-admin">ADMIN</span></span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/admin/users/</span><span class="param">{userId}</span><span class="seg">/</span><span class="action">unban</span></span><span class="desc">Restore access<span class="tag tag-admin">ADMIN</span></span></div>
    <div class="endpoint"><span class="method m-PATCH">PATCH</span><span class="route"><span class="seg">/admin/users/</span><span class="param">{userId}</span><span class="seg">/role</span></span><span class="desc">Change user role<span class="tag tag-admin">ADMIN</span></span></div>
    <div class="group-label">Content moderation</div>
    <div class="endpoint"><span class="method m-DELETE">DELETE</span><span class="route"><span class="seg">/admin/posts/</span><span class="param">{postId}</span></span><span class="desc">Remove post<span class="tag tag-admin">ADMIN</span></span></div>
    <div class="endpoint"><span class="method m-DELETE">DELETE</span><span class="route"><span class="seg">/admin/reviews/</span><span class="param">{reviewId}</span></span><span class="desc">Remove review<span class="tag tag-admin">ADMIN</span></span></div>
    <div class="endpoint"><span class="method m-POST">POST</span><span class="route"><span class="seg">/admin/clubs/</span><span class="param">{clubId}</span><span class="seg">/</span><span class="action">suspend</span></span><span class="desc">Suspend club<span class="tag tag-admin">ADMIN</span></span></div>
    <div class="group-label">Platform stats</div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/admin/stats/overview</span></span><span class="desc">Platform KPIs<span class="tag tag-admin">ADMIN</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/admin/stats/bookings</span></span><span class="desc">Booking analytics<span class="tag tag-admin">ADMIN</span><span class="tag tag-search">?from= ?to=</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/admin/stats/revenue</span></span><span class="desc">Revenue report<span class="tag tag-admin">ADMIN</span><span class="tag tag-search">?from= ?to=</span></span></div>
    <div class="endpoint"><span class="method m-GET">GET</span><span class="route"><span class="seg">/admin/stats/signups</span></span><span class="desc">Growth metrics<span class="tag tag-admin">ADMIN</span><span class="tag tag-search">?from= ?to=</span></span></div>
  </div>

</main>

<script>
const sections = document.querySelectorAll('.section');
const navItems = document.querySelectorAll('.nav-item');
const obs = new IntersectionObserver(entries => {
  entries.forEach(e => {
    if (e.isIntersecting) {
      navItems.forEach(n => n.classList.remove('active'));
      const active = document.querySelector('.nav-item[href="#' + e.target.id + '"]');
      if (active) active.classList.add('active');
    }
  });
}, { threshold: 0.3 });
sections.forEach(s => obs.observe(s));
</script>
</body>
</html>
```

## File: .gitignore
```
appsettings.json
bin/
obj/
.vs/
*.user
wwwroot/uploads/
```

## File: appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },

  "AllowedHosts": "*",

  "ConnectionStrings": {
    "DefaultConnection": "Data Source=.\\Ahmed1;Initial Catalog=ssss;Integrated Security=True;Encrypt=True;Trust Server Certificate=True",
    "HangfireConnection": "Data Source=.\\Ahmed1;Initial Catalog=ssss;Integrated Security=True;Encrypt=True;Trust Server Certificate=True"
  },

  "Jwt": {
    "Key": "jjkjfhhhjfbghbdfgbdfgbhbdfhgbdhbjvhbk",
    "Issuer": "SurveyBasketApp",
    "Audience": "SurveyBasketApp users",
    "ExpiryMinutes": 30
  },

  "AppSettings": {
    "FrontendOrigin": "https://front-end-project-bay-seven.vercel.app"
  },

  "Authentication": {
    "Google": {
      "ClientId": "1018203917478-m61lfh6qdo2uv1mqf59qc2osue4el2l9.apps.googleusercontent.com",
      "ClientSecret": "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",
      "RedirectUri": "/signin-google",
      "Scopes": [ "openid", "profile", "email" ]
    },
    "GitHub": {
      "ClientId": "Ov23liZQDMerjBEdPB71",
      "ClientSecret": "0ac4f14d1a9b4d4b9e96194763bc53da11b5de0c",
      "RedirectUri": "/signin-github",
      "Scopes": [ "user:email", "read:user" ]
    }
  },

  "MailSettings": {
    "Mail": "sayed732004444@gmail.com",
    "DisplayName": "Ahmed Elsayed",
    "Password": "yxva ikie aqnm obix",
    "Host": "smtp.gmail.com",
    "Port": 587
  },

  "AllowedOrigins": [
    "https://front-end-project-bay-seven.vercel.app",
    "https://careerpathfinal.runasp.net",
    "http://localhost:5173",
    "http://localhost:3000",
    "https://localhost:7283",
    "http://localhost:5250"
  ],

  "AdzunaApi": {
    "BaseUrl": "https://api.adzuna.com/v1/api",
    "AppId": "0c2dc806",
    "AppKey": "9c221969b8d228069a84d16ac3b204ce"
  },

  "HangfireSettings": {
    "Username": "admin",
    "Password": "admin"
  }
}
```

## File: Authentication/JwtProvider.cs
```csharp
namespace Sportiva.Authentication;

public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
{
    private readonly JwtOptions _options = options.Value;

    public (string token, int expiresIn) GenerateToken(ApplicationUser user, IEnumerable<string> roles, IEnumerable<string> permissions)
    {
        Claim[] claims = [
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new(JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString()),
            new(nameof(roles), JsonSerializer.Serialize(roles), JsonClaimValueTypes.JsonArray),
            new(nameof(permissions), JsonSerializer.Serialize(permissions), JsonClaimValueTypes.JsonArray)
        ];

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));

        var singingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.ExpiryMinutes),
            signingCredentials: singingCredentials
        );

        return (token: new JwtSecurityTokenHandler().WriteToken(token), expiresIn: _options.ExpiryMinutes * 60);
    }

    public string? ValidateToken(string token, bool validateLifetime = true)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                IssuerSigningKey = symmetricSecurityKey,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = validateLifetime, // ← السطر ده بس
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            return jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
        }
        catch
        {
            return null;
        }
    }

}
```

## File: Contracts/Clubs/CreateClubRequest.cs
```csharp
namespace Sportiva.Contracts.Clubs;

public record CreateClubRequest(
    string? Name,
    IFormFile? Logo,
    string? Governorate,
    string? City,
    string? Address,
    string? PhoneNumber,
    string? Email
);
```

## File: Contracts/Clubs/UpdateClubRequest.cs
```csharp
namespace Sportiva.Contracts.Clubs;

public record UpdateClubRequest(
    string? Name,
    IFormFile? Logo,
    string? Governorate,
    string? City,
    string? Address,
    string? PhoneNumber,
    string? Email,
    bool IsActive
);
```

## File: Contracts/Courts/CreateCourtRequest.cs
```csharp
using Sportiva.Contracts.Shared.Enums;

namespace Sportiva.Contracts.Courts;

public record CreateCourtRequest(
    string ClubId,
    string? Name,
    string? Description,
    IFormFile? Image,
    SportTypeDto SportType,
    int MaxCapacity,
    decimal PricePerHour
);
```

## File: Contracts/Courts/UpdateCourtRequest.cs
```csharp
using Sportiva.Contracts.Shared.Enums;

namespace Sportiva.Contracts.Courts;

public record UpdateCourtRequest(
    string? Name,
    string? Description,
    IFormFile? Image,
    SportTypeDto SportType,
    int MaxCapacity,
    decimal PricePerHour,
    bool IsActive
);
```

## File: DependencyInjection.cs
```csharp
using Hangfire;
using Microsoft.AspNetCore.Authentication.Cookies;
using Sportiva.Services;

namespace Sportiva;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add<CancellationExceptionFilter>();
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(
                new JsonStringEnumConverter()
            );
        });

        services.AddOpenApi();

        services.AddCors(options =>
            options.AddDefaultPolicy(builder =>
                builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithOrigins(
                    "http://localhost:5173",
                    "https://front-end-project-bay-seven.vercel.app"
                        )
                .AllowCredentials()
            )
        );

        services.AddAuthConfig(configuration);

        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services
            .AddMapsterConfig()
            .AddFluentValidationConfig();
        services.AddSignalR();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailSender, EmailService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IClubService, ClubService>();
        services.AddHttpClient();
        services.AddHttpContextAccessor();
        services.AddBackgroundJobsConfig(configuration);

        services.AddOptions<MailSettings>()
            .BindConfiguration(nameof(MailSettings))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    // ==================== Mapster ====================
    private static IServiceCollection AddMapsterConfig(this IServiceCollection services)
    {
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton<IMapper>(new Mapper(mappingConfig));
        return services;
    }

    // ==================== FluentValidation ====================
    private static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }

    // ==================== AUTH CONFIG ====================
    private static IServiceCollection AddAuthConfig(this IServiceCollection services,
 IConfiguration configuration)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
        services.AddSingleton<IJwtProvider, JwtProvider>();

        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var jwtSettings = configuration
            .GetSection(JwtOptions.SectionName)
            .Get<JwtOptions>();

        // ── Read OAuth config ───────────────────────────────────────────────
        var googleConfig = configuration
            .GetSection(GoogleOAuthOptions.SectionName)
            .Get<GoogleOAuthOptions>();

        var githubConfig = configuration
            .GetSection(GitHubOAuthOptions.SectionName)
            .Get<GitHubOAuthOptions>();

        // ── Bind options so they can be injected anywhere via IOptions<T> ──
        services.Configure<GoogleOAuthOptions>(
            configuration.GetSection(GoogleOAuthOptions.SectionName));

        services.Configure<GitHubOAuthOptions>(
            configuration.GetSection(GitHubOAuthOptions.SectionName));

        // ── Authentication pipeline ─────────────────────────────────────────
        var authBuilder = services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings!.Key)),
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience
            };

            // ── SignalR JWT from Query String ───────────────────────────────
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;

                    if (!string.IsNullOrEmpty(accessToken) &&
                        path.StartsWithSegments("/hubs"))
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }
            };
        });

        // ── Google OAuth (only if configured) ──────────────────────────────
        if (!string.IsNullOrWhiteSpace(googleConfig?.ClientId) &&
            !string.IsNullOrWhiteSpace(googleConfig?.ClientSecret))
        {
            authBuilder.AddGoogle(options =>
            {
                options.ClientId = googleConfig.ClientId;
                options.ClientSecret = googleConfig.ClientSecret;
                options.SaveTokens = true;

                if (!string.IsNullOrWhiteSpace(googleConfig.RedirectUri))
                    options.CallbackPath = googleConfig.RedirectUri;

                foreach (var scope in googleConfig.Scopes ?? ["email", "profile"])
                    options.Scope.Add(scope);
            });
        }

        // ── GitHub OAuth (only if configured) ──────────────────────────────
        if (!string.IsNullOrWhiteSpace(githubConfig?.ClientId) &&
            !string.IsNullOrWhiteSpace(githubConfig?.ClientSecret))
        {
            authBuilder.AddGitHub(options =>
            {
                options.ClientId = githubConfig.ClientId;
                options.ClientSecret = githubConfig.ClientSecret;
                options.CallbackPath = "/signin-github";
                options.SaveTokens = true;

                foreach (var scope in githubConfig.Scopes ?? ["user:email"])
                    options.Scope.Add(scope);
            });
        }

        // ── Prevent cookie redirects on API endpoints → return 401/403 ─────
        services.ConfigureApplicationCookie(options =>
        {
            options.Events = new Microsoft.AspNetCore.Authentication.Cookies
                .CookieAuthenticationEvents
            {
                OnRedirectToLogin = ctx =>
                {
                    if (ctx.Request.Path.StartsWithSegments("/api") ||
                        ctx.Request.Headers["Accept"].ToString()
                           .Contains("application/json"))
                    {
                        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    }
                    ctx.Response.Redirect(ctx.RedirectUri);
                    return Task.CompletedTask;
                },
                OnRedirectToAccessDenied = ctx =>
                {
                    if (ctx.Request.Path.StartsWithSegments("/api") ||
                        ctx.Request.Headers["Accept"].ToString()
                           .Contains("application/json"))
                    {
                        ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return Task.CompletedTask;
                    }
                    ctx.Response.Redirect(ctx.RedirectUri);
                    return Task.CompletedTask;
                }
            };
        });

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequiredLength = 8;
            options.SignIn.RequireConfirmedEmail = true;
            options.User.RequireUniqueEmail = true;
        });

        return services;
    }
    // ==================== Hangfire ====================
    private static IServiceCollection AddBackgroundJobsConfig(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(
                configuration.GetConnectionString("HangfireConnection")));

        services.AddHangfireServer();

        return services;
    }
}
```

## File: Persistence/ApplicationDbContext.cs
```csharp
namespace Sportiva.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
{
    public DbSet<UserProfile> UserProfiles { get; set; } = default!;
    public DbSet<Post> Posts { get; set; } = default!;
    public DbSet<PostLike> PostLikes { get; set; } = default!;
    public DbSet<Club> Clubs { get; set; } = default!;
    public DbSet<Court> Courts { get; set; } = default!;
    public DbSet<TimeSlot> TimeSlots { get; set; } = default!;
    public DbSet<Booking> Bookings { get; set; } = default!;
    public DbSet<Review> Reviews { get; set; } = default!;
    public DbSet<FriendlyMatch> FriendlyMatches { get; set; } = default!;
    public DbSet<MatchJoinRequest> MatchJoinRequests { get; set; } = default!;
    public DbSet<ClubSubscription> ClubSubscriptions { get; set; } = default!;
    public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; } = default!;
    public DbSet<MembershipUpgrade> MembershipUpgrades { get; set; } = default!;
    public DbSet<Message> Messages { get; set; } = default!;
    public DbSet<Notification> Notifications { get; set; } = default!;
    public DbSet<NotificationPreference> NotificationPreferences { get; set; } = default!;
    public DbSet<Tournament> Tournaments { get; set; } = default!;
    public DbSet<TournamentMatch> TournamentMatches { get; set; } = default!;
    public DbSet<TournamentParticipant> TournamentParticipants { get; set; } = default!;
    public DbSet<PostComment> PostComments { get; set; } = default!;
    public DbSet<CommentReaction> CommentReactions { get; set; } = default!;
    public DbSet<CommentReply> CommentReplies { get; set; } = default!;
    public DbSet<ReplyReaction> ReplyReactions { get; set; } = default!;
    public DbSet<UserFollow> UserFollows { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        var cascadeFKs = modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade && !fk.IsOwnership);

        foreach (var fk in cascadeFKs)
            fk.DeleteBehavior = DeleteBehavior.Restrict;

        base.OnModelCreating(modelBuilder);
    }
}
```

## File: Persistence/EntitiesConfigurations/BookingConfiguration.cs
```csharp
namespace Sportiva.Persistence.EntitiesConfigurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Price)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(x => x.Status)
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasIndex(x => new { x.UserId, x.BookingDate });

        builder.HasOne(x => x.Court)
               .WithMany()
               .HasForeignKey(x => x.CourtId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TimeSlot)
               .WithMany(t => t.Bookings)
               .HasForeignKey(x => x.TimeSlotId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
               .WithMany(u => u.Bookings)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
```

## File: Persistence/EntitiesConfigurations/DefaultRoles.cs
```csharp
namespace Sportiva.Persistence.EntitiesConfigurations; // ✅ تم تصحيح الـ namespace من Sportiva.Abstractions.Consts

public static class DefaultRoles
{
    public partial class Admin
    {
        public const string Name = nameof(Admin);
        public const string Id = "0191a4b6-c4fc-752e-9d95-40b5e4e68054";
        public const string ConcurrencyStamp = "0191a4b6-c4fc-752e-9d95-40b631d1866d";
    }

    // ✅ تم حذف Role الـ Company لأنه مش متضاف في RoleConfiguration ولا في الـ Database Seed
    // لو محتاجه، أضفه في RoleConfiguration بـ HasData

    public partial class Member
    {
        public const string Name = nameof(Member);
        public const string Id = "0191a4b6-c4fc-752e-9d95-40b7a5cb88f0";
        public const string ConcurrencyStamp = "0191a4b6-c4fc-752e-9d95-40b85cf3fd22";
    }
}
```

## File: Persistence/EntitiesConfigurations/PostConfiguration.cs
```csharp
namespace Sportiva.Persistence.EntitiesConfigurations;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Content)
               .HasMaxLength(2000)
               .IsRequired();

        builder.Property(x => x.FileUrl).HasMaxLength(500);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne(x => x.User)
               .WithMany(p => p.Posts)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Likes)
               .WithOne(l => l.Post)
               .HasForeignKey(l => l.PostId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Comments)
               .WithOne(c => c.Post)
               .HasForeignKey(c => c.PostId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
```

## File: Persistence/EntitiesConfigurations/RoleClaimConfiguration.cs
```csharp
namespace Sportiva.Persistence.EntitiesConfigurations;

public class RoleClaimConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<string>>
{
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
    {
        //Default Data
        var permissions = Permissions.GetAllPermissions();
        var adminClaims = new List<IdentityRoleClaim<string>>();

        for (var i = 0; i < permissions.Count; i++)
        {
            adminClaims.Add(new IdentityRoleClaim<string>
            {
                Id = i + 1,
                ClaimType = Permissions.Type,
                ClaimValue = permissions[i],
                RoleId = DefaultRoles.Admin.Id
            });
        }

        builder.HasData(adminClaims);
    }
}
```

## File: Persistence/EntitiesConfigurations/RoleConfiguration.cs
```csharp
namespace Sportiva.Persistence.EntitiesConfigurations;

public class RoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        //Default Data
        builder.HasData([
            new ApplicationRole
            {
                Id = DefaultRoles.Admin.Id,
                Name = DefaultRoles.Admin.Name,
                NormalizedName = DefaultRoles.Admin.Name.ToUpper(),
                ConcurrencyStamp = DefaultRoles.Admin.ConcurrencyStamp
            },
            new ApplicationRole
            {
                Id = DefaultRoles.Member.Id,
                Name = DefaultRoles.Member.Name,
                NormalizedName = DefaultRoles.Member.Name.ToUpper(),
                ConcurrencyStamp = DefaultRoles.Member.ConcurrencyStamp,
                IsDefault = true
            }
        ]);
    }
}
```

## File: Persistence/EntitiesConfigurations/UserConfiguration.cs
```csharp
namespace Sportiva.Persistence.EntitiesConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasMany(x => x.RefreshTokens)
            .WithOne()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.RefreshTokens).AutoInclude(false);

        builder.Property(x => x.FirstName).HasMaxLength(100);
        builder.Property(x => x.LastName).HasMaxLength(100);

        builder.HasMany(x => x.Following)
            .WithOne(f => f.Follower)
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Followers)
            .WithOne(f => f.Following)
            .HasForeignKey(f => f.FollowingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(new ApplicationUser
        {
            Id = DefaultUsers.Admin.Id,
            FirstName = "Sportiva",
            LastName = "Admin",
            UserName = DefaultUsers.Admin.Email,
            NormalizedUserName = DefaultUsers.Admin.Email.ToUpper(),
            Email = DefaultUsers.Admin.Email,
            NormalizedEmail = DefaultUsers.Admin.Email.ToUpper(),
            SecurityStamp = DefaultUsers.Admin.SecurityStamp,
            ConcurrencyStamp = DefaultUsers.Admin.ConcurrencyStamp,
            EmailConfirmed = true,
            PasswordHash = DefaultUsers.Admin.PasswordHash,
            CreatedAt = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}
```

## File: Persistence/EntitiesConfigurations/UserProfileConfiguration.cs
```csharp
namespace Sportiva.Persistence.EntitiesConfigurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Bio).HasMaxLength(500);
        builder.Property(x => x.City).HasMaxLength(100);
        builder.Property(x => x.Country).HasMaxLength(100);
        builder.Property(x => x.ProfilePictureUrl).HasMaxLength(500);
        builder.Property(x => x.CoverImageUrl).HasMaxLength(500);
        builder.Property(x => x.PreferredCity).HasMaxLength(100);

        builder.Property(x => x.PreferredSport)
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasIndex(x => x.UserId).IsUnique();

        builder.HasOne(x => x.User)
               .WithOne(x => x.UserProfile)
               .HasForeignKey<UserProfile>(x => x.UserId);

    }
}
```

## File: repomix.config.json
```json
{
  "output": {
    "filePath": "ai-context.md",
    "style": "markdown"
  },
  "ignore": {
    "customPatterns": [
      "ai-context.md",
      "repomix-output.xml",
      "keys/**",
      "wwwroot/**",
      "**/*.xml",
      "**/*.csproj",
      "**/*.sln",
      "**/*.user",
      "**/*.designer.cs",
      "**/*.g.cs",
      "**/bin/**",
      "**/obj/**",
      "**/.vs/**",
      "**/Migrations/**"
    ]
  }
}

//repomix --include "Persistence/**,Entities/**,Services/**,Errors/**,Contracts/**"
//repomix --include "Contracts/**"
//repomix --include "Contracts/Posts/**,Controllers/PostsController.cs,Controllers/CommentsController.cs,Services/IPostService.cs,Services/ICommentService.cs,Errors/PostErrors.cs,Errors/CommentErrors.cs"
//tree /F /A > tree.txt
```

## File: Services/Abstraction/IBookingService.cs
```csharp
using Sportiva.Contracts.Bookings;
using Sportiva.Contracts.Common;
using Sportiva.Enums;

namespace Sportiva.Services;

public interface IBookingService
{
    Task<Result<BookingResponse>> GetBookingAsync(
        string bookingId, string currentUserId, CancellationToken ct = default);

    Task<Result<PaginatedList<BookingResponse>>> GetMyBookingsAsync(
        string userId, RequestFilters filters, BookingStatus? status = null, CancellationToken ct = default);

    Task<Result<PaginatedList<BookingResponse>>> GetCourtBookingsAsync(
        string userId, string courtId, RequestFilters filters, DateOnly? date = null, CancellationToken ct = default);

    Task<Result<PaginatedList<BookingResponse>>> GetClubBookingsAsync(
        string userId, string clubId, RequestFilters filters, CancellationToken ct = default);

    Task<Result<BookingResponse>> CreateBookingAsync(
        string userId, CreateBookingRequest request, CancellationToken ct = default);

    Task<Result> CancelBookingAsync(
        string userId, string bookingId, CancellationToken ct = default);

    Task<Result<BookingResponse>> GetBookingReceiptAsync(
        string userId, string bookingId, CancellationToken ct = default);
}
```

## File: Services/Abstraction/IClubService.cs
```csharp
using Sportiva.Contracts.Clubs;
using Sportiva.Contracts.Common;

namespace Sportiva.Services;

public interface IClubService
{
    Task<Result<ClubResponse>> GetClubAsync(
        string clubId, string? currentUserId = null, CancellationToken ct = default);

    Task<Result<PaginatedList<ClubResponse>>> GetClubsAsync(
        string? currentUserId, RequestFilters filters, CancellationToken ct = default);

    Task<Result<ClubResponse>> CreateClubAsync(
        string ownerId, CreateClubRequest request, CancellationToken ct = default);

    Task<Result<ClubResponse>> UpdateClubAsync(
        string userId, string clubId, UpdateClubRequest request, CancellationToken ct = default);
    //soft delete
    Task<Result> DeleteClubAsync(
        string userId, string clubId, CancellationToken ct = default);

    Task<Result> ToggleClubStatusAsync(
        string userId, string clubId, CancellationToken ct = default);

    Task<Result<PaginatedList<ClubResponse>>> GetMyClubsAsync(
        string userId, RequestFilters filters, CancellationToken ct = default);
}
```

## File: Services/Abstraction/IClubSubscriptionService.cs
```csharp
using Sportiva.Contracts.Common;
using Sportiva.Contracts.Subscriptions;

namespace Sportiva.Services;

public interface IClubSubscriptionService
{
    Task<Result<ClubSubscriptionResponse>> GetActiveSubscriptionAsync(
        string userId, string clubId, CancellationToken ct = default);

    Task<Result<PaginatedList<ClubSubscriptionResponse>>> GetSubscriptionHistoryAsync(
        string userId, string clubId, RequestFilters filters, CancellationToken ct = default);

    Task<Result<ClubSubscriptionResponse>> SubscribeAsync(
        string userId, string clubId, CreateClubSubscriptionRequest request, CancellationToken ct = default);

    Task<Result<ClubSubscriptionResponse>> RenewSubscriptionAsync(
        string userId, string clubId, CancellationToken ct = default);

    Task<Result> CancelSubscriptionAsync(
        string userId, string clubId, CancellationToken ct = default);
}
```

## File: Services/Abstraction/ICourtService.cs
```csharp
using Sportiva.Contracts.Common;
using Sportiva.Contracts.Courts;
using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Services;

public interface ICourtService
{
    Task<Result<PaginatedList<CourtResponse>>> SearchCourtsAsync(
        string? currentUserId, RequestFilters filters,
        SportType? sport = null, string? city = null, DateOnly? date = null,
        CancellationToken ct = default);

    Task<Result<PaginatedList<CourtResponse>>> GetClubCourtsAsync(
        string clubId, string? currentUserId, RequestFilters filters, CancellationToken ct = default);

    Task<Result<CourtResponse>> GetCourtAsync(
        string clubId, string courtId, string? currentUserId = null, CancellationToken ct = default);

    Task<Result<CourtResponse>> CreateCourtAsync(
        string userId, string clubId, CreateCourtRequest request, CancellationToken ct = default);

    Task<Result<CourtResponse>> UpdateCourtAsync(
        string userId, string clubId, string courtId, UpdateCourtRequest request, CancellationToken ct = default);
    //soft delete 
    Task<Result> DeleteCourtAsync(
        string userId, string clubId, string courtId, CancellationToken ct = default);

    Task<Result> ToggleCourtStatusAsync(
        string userId, string clubId, string courtId, CancellationToken ct = default);

    Task<Result<IReadOnlyList<TimeSlotSummary>>> GetCourtAvailabilityAsync(
        string courtId, DateOnly date, CancellationToken ct = default);
}
```

## File: Services/Abstraction/IFriendlyMatchService.cs
```csharp
using Sportiva.Contracts.Common;
using Sportiva.Contracts.Matches;
using Sportiva.Contracts.Shared.Summaries;
using Sportiva.Enums;

namespace Sportiva.Services;

public interface IFriendlyMatchService
{
    Task<Result<PaginatedList<FriendlyMatchResponse>>> GetMatchesAsync(
        string? currentUserId, RequestFilters filters,
        SportType? sport = null, DateOnly? date = null, string? city = null,
        CancellationToken ct = default);

    Task<Result<FriendlyMatchResponse>> GetMatchAsync(
        string matchId, string? currentUserId = null, CancellationToken ct = default);

    Task<Result<FriendlyMatchResponse>> CreateMatchAsync(
        string userId, CreateFriendlyMatchRequest request, CancellationToken ct = default);

    Task<Result<FriendlyMatchResponse>> UpdateMatchAsync(
        string userId, string matchId, CreateFriendlyMatchRequest request, CancellationToken ct = default);

    Task<Result> CancelMatchAsync(
        string userId, string matchId, CancellationToken ct = default);

    Task<Result<IReadOnlyList<ParticipantSummary>>> GetParticipantsAsync(
        string matchId, CancellationToken ct = default);

    Task<Result> LeaveMatchAsync(
        string userId, string matchId, CancellationToken ct = default);

    Task<Result<PaginatedList<FriendlyMatchResponse>>> GetMyMatchesAsync(
        string userId, RequestFilters filters, string? role = null, CancellationToken ct = default);

    Task<Result<PaginatedList<FriendlyMatchResponse>>> GetCourtMatchesAsync(
        string courtId, RequestFilters filters, CancellationToken ct = default);
}
```

## File: Services/Abstraction/IMembershipUpgradeService.cs
```csharp
using Sportiva.Contracts.Common;
using Sportiva.Contracts.Memberships;
using Sportiva.Enums;

namespace Sportiva.Services;

public interface IMembershipUpgradeService
{
    Task<Result<MembershipUpgradeResponse>> GetUpgradeRequestAsync(
        string requestId, CancellationToken ct = default);

    Task<Result<PaginatedList<MembershipUpgradeResponse>>> GetUpgradeRequestsAsync(
        RequestFilters filters, RequestStatus? status = null, CancellationToken ct = default);

    Task<Result<MembershipUpgradeResponse>> GetMyUpgradeRequestAsync(
        string userId, CancellationToken ct = default);

    Task<Result<MembershipUpgradeResponse>> SubmitUpgradeRequestAsync(
        string userId, CreateMembershipUpgradeRequest request, CancellationToken ct = default);

    Task<Result> ReviewUpgradeRequestAsync(
        string adminId, string requestId, ReviewMembershipUpgradeRequest request, CancellationToken ct = default);
}
```

## File: Services/Abstraction/IMessagingService.cs
```csharp
using Sportiva.Contracts.Common;
using Sportiva.Contracts.Messaging;

namespace Sportiva.Services;

public interface IMessagingService
{
    Task<Result<PaginatedList<ConversationSummary>>> GetConversationsAsync(
        string userId, RequestFilters filters, CancellationToken ct = default);

    Task<Result<PaginatedList<MessageResponse>>> GetMessagesAsync(
        string userId, string otherUserId, RequestFilters filters, CancellationToken ct = default);

    Task<Result<MessageResponse>> SendMessageAsync(
        string senderId, SendMessageRequest request, CancellationToken ct = default);

    Task<Result> MarkConversationAsReadAsync(
        string userId, string otherUserId, CancellationToken ct = default);

    Task<Result> DeleteMessageAsync(
        string userId, string messageId, CancellationToken ct = default);
}
```

## File: Services/Abstraction/INotificationService.cs
```csharp
namespace Sportiva.Services;

public interface INotificationService
{
    //Task<Result<NotificationListResponse>> GetNotificationsAsync(
    //    string userId, int pageNumber, int pageSize, CancellationToken ct = default);

    //Task<Result<NotificationCountersResponse>> GetNotificationCountersAsync(
    //    string userId, CancellationToken ct = default);

    //Task<Result> MarkAsReadAsync(
    //    string userId, string notificationId, CancellationToken ct = default);

    //Task<Result> MarkAllAsReadAsync(
    //    string userId, CancellationToken ct = default);

    //Task<Result<NotificationPreferencesListResponse>> GetPreferencesAsync(
    //    string userId, CancellationToken ct = default);

    //Task<Result> UpdatePreferencesAsync(
    //    string userId, BulkUpdateNotificationPreferencesRequest request, CancellationToken ct = default);

    //Task SendNotificationAsync(
    //    string recipientId, NotificationType type, string title, string body,
    //    string? actorId = null, string? entityType = null, string? entityId = null,
    //    NotificationPriority priority = NotificationPriority.Normal,
    //    CancellationToken ct = default);
}
```

## File: Services/Abstraction/IReviewService.cs
```csharp
using Sportiva.Contracts.Common;
using Sportiva.Contracts.Reviews;

namespace Sportiva.Services;

public interface IReviewService
{
    Task<Result<ReviewResponse>> GetReviewAsync(
        string reviewId, string? currentUserId = null, CancellationToken ct = default);

    Task<Result<ReviewResponse>> GetBookingReviewAsync(
        string userId, string bookingId, CancellationToken ct = default);

    Task<Result<PaginatedList<ReviewResponse>>> GetCourtReviewsAsync(
        string courtId, string? currentUserId, RequestFilters filters, CancellationToken ct = default);

    Task<Result<PaginatedList<ReviewResponse>>> GetClubReviewsAsync(
        string clubId, string? currentUserId, RequestFilters filters, CancellationToken ct = default);

    Task<Result<PaginatedList<ReviewResponse>>> GetMyReviewsAsync(
        string userId, RequestFilters filters, CancellationToken ct = default);

    Task<Result<ReviewResponse>> CreateReviewAsync(
        string userId, CreateReviewRequest request, CancellationToken ct = default);

    Task<Result<ReviewResponse>> UpdateReviewAsync(
        string userId, string reviewId, CreateReviewRequest request, CancellationToken ct = default);

    Task<Result> DeleteReviewAsync(
        string userId, string reviewId, CancellationToken ct = default);
}
```

## File: Services/Abstraction/ISubscriptionPlanService.cs
```csharp
using Sportiva.Contracts.Subscriptions;

namespace Sportiva.Services;

public interface ISubscriptionPlanService
{
    Task<Result<IReadOnlyList<SubscriptionPlanResponse>>> GetPlansAsync(
        CancellationToken ct = default);

    Task<Result<SubscriptionPlanResponse>> GetPlanAsync(
        string planId, CancellationToken ct = default);

    Task<Result<SubscriptionPlanResponse>> CreatePlanAsync(
        CreateClubSubscriptionRequest request, CancellationToken ct = default);

    Task<Result<SubscriptionPlanResponse>> UpdatePlanAsync(
        string planId, CreateClubSubscriptionRequest request, CancellationToken ct = default);

    Task<Result> ArchivePlanAsync(
        string planId, CancellationToken ct = default);
}
```

## File: Services/Abstraction/ITimeSlotService.cs
```csharp
using Sportiva.Contracts.TimeSlots;

namespace Sportiva.Services;

public interface ITimeSlotService
{
    Task<Result<IReadOnlyList<TimeSlotResponse>>> GetTimeSlotsAsync(
        string courtId, DateOnly? date = null, bool? available = null, CancellationToken ct = default);

    Task<Result<TimeSlotResponse>> GetTimeSlotAsync(
        string courtId, string slotId, CancellationToken ct = default);

    Task<Result<TimeSlotResponse>> CreateTimeSlotAsync(
        string userId, string courtId, CreateTimeSlotRequest request, CancellationToken ct = default);

    Task<Result<IReadOnlyList<TimeSlotResponse>>> BulkCreateTimeSlotsAsync(
        string userId, string courtId, IReadOnlyList<CreateTimeSlotRequest> requests, CancellationToken ct = default);

    Task<Result<TimeSlotResponse>> UpdateTimeSlotAsync(
        string userId, string courtId, string slotId, CreateTimeSlotRequest request, CancellationToken ct = default);

    Task<Result> DeleteTimeSlotAsync(
        string userId, string courtId, string slotId, CancellationToken ct = default);
}
```

## File: Services/Abstraction/ITournamentService.cs
```csharp
namespace Sportiva.Services;

public interface ITournamentService
{
    // ── Tournaments ──────────────────────────────────────────────────────────

    //Task<Result<PaginatedList<TournamentResponse>>> GetTournamentsAsync(
    //    string? currentUserId, RequestFilters filters,
    //    SportType? sport = null, TournamentStatus? status = null, string? city = null,
    //    CancellationToken ct = default);

    //Task<Result<TournamentResponse>> GetTournamentAsync(
    //    string tournamentId, string? currentUserId = null, CancellationToken ct = default);

    //Task<Result<TournamentResponse>> CreateTournamentAsync(
    //    string userId, CreateTournamentRequest request, CancellationToken ct = default);

    //Task<Result<TournamentResponse>> UpdateTournamentAsync(
    //    string userId, string tournamentId, CreateTournamentRequest request, CancellationToken ct = default);

    //Task<Result> CancelTournamentAsync(
    //    string userId, string tournamentId, CancellationToken ct = default);

    //Task<Result> JoinTournamentAsync(
    //    string userId, string tournamentId, CancellationToken ct = default);

    //Task<Result> LeaveTournamentAsync(
    //    string userId, string tournamentId, CancellationToken ct = default);

    //Task<Result<PaginatedList<ParticipantSummary>>> GetTournamentParticipantsAsync(
    //    string tournamentId, RequestFilters filters, CancellationToken ct = default);

    //Task<Result<PaginatedList<TournamentResponse>>> GetMyTournamentsAsync(
    //    string userId, RequestFilters filters, CancellationToken ct = default);

    //// ── Tournament Matches (bracket) ─────────────────────────────────────────

    //Task<Result<IReadOnlyList<TournamentMatchResponse>>> GetTournamentMatchesAsync(
    //    string tournamentId, int? round = null, CancellationToken ct = default);

    //Task<Result<TournamentMatchResponse>> GetTournamentMatchAsync(
    //    string tournamentId, string matchId, CancellationToken ct = default);

    //Task<Result<TournamentMatchResponse>> ScheduleTournamentMatchAsync(
    //    string userId, string tournamentId, CreateTournamentMatchRequest request, CancellationToken ct = default);

    //Task<Result<TournamentMatchResponse>> UpdateTournamentMatchAsync(
    //    string userId, string tournamentId, string matchId, CreateTournamentMatchRequest request, CancellationToken ct = default);

    //Task<Result> CancelTournamentMatchAsync(
    //    string userId, string tournamentId, string matchId, CancellationToken ct = default);

    //Task<Result<TournamentMatchResponse>> SetMatchWinnerAsync(
    //    string userId, string tournamentId, string matchId, SetTournamentMatchWinnerRequest request, CancellationToken ct = default);
}
```

## File: tree.txt
```
Folder PATH listing
Volume serial number is 0000024E 28EE:D16A
C:\USERS\AIO\SOURCE\REPOS\SPORTIVAAPI\SPORTIVA\SRC
Invalid path - \USERS\AIO\SOURCE\REPOS\SPORTIVAAPI\SPORTIVA\SRC
No subfolders exist
```

## File: Program.cs
```csharp
using Microsoft.AspNetCore.HttpOverrides;
using Sportiva;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencies(builder.Configuration);

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
//app.MapOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "careerPath V1");
});

app.MapControllers();

app.Run();



//IClubService — كل حاجة بتتعلق بيه
//ISubscriptionPlanService — الـ plans لازم تتعمل قبل الـ club subscriptions
//IClubSubscriptionService — بعد الـ plans
//ICourtService — بيتبع الـ club
//ITimeSlotService — بيتبع الـ court
//IBookingService — محتاج court + time slot
//IReviewService — محتاج booking
//IMembershipUpgradeService — مستقل نسبياً
//IFriendlyMatchService — محتاج court
//IMatchJoinRequestService — بيتبع الـ match
//ITournamentService — أضخم feature، بيتبع court كمان
//INotificationService — cross-cutting، يتعمل قبل ما تشتغل على الـ real-time features
//IMessagingService — آخر حاجة، مستقلة تماماً
```
