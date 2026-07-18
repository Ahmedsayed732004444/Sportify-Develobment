using Sportiva.Contracts.Common;
using Sportiva.Contracts.Matches;
using Sportiva.Contracts.Shared.Enums;
using Sportiva.Contracts.Shared.Summaries;
using Sportiva.Abstractions;

namespace Sportiva.Services;

public class MatchJoinRequestService(
    ApplicationDbContext context,
    INotificationService notificationService,
    ILogger<MatchJoinRequestService> logger) : IMatchJoinRequestService
{
    private readonly ApplicationDbContext _context = context;
    private readonly INotificationService _notificationService = notificationService;
    private readonly ILogger<MatchJoinRequestService> _logger = logger;

    private static MatchJoinRequestResponse MapToResponse(MatchJoinRequest request, string userId)
    {
        var courtSummary = new CourtSummary(
            request.FriendlyMatch.Court.Id,
            request.FriendlyMatch.Court.Name,
            request.FriendlyMatch.Court.ImageUrl,
            (SportTypeDto)request.FriendlyMatch.Court.SportType,
            request.FriendlyMatch.Court.PricePerHour,
            new ClubSummary(
                request.FriendlyMatch.Court.Club.Id,
                request.FriendlyMatch.Court.Club.Name,
                request.FriendlyMatch.Court.Club.LogoUrl,
                request.FriendlyMatch.Court.Club.City,
                request.FriendlyMatch.Court.Club.Governorate)
        );

        var matchSummary = new FriendlyMatchSummary(
            request.FriendlyMatch.Id,
            request.FriendlyMatch.Date,
            request.FriendlyMatch.StartTime,
            request.FriendlyMatch.EndTime,
            (SportTypeDto)request.FriendlyMatch.SportType,
            courtSummary
        );

        var playerSummary = new UserSummary(
            request.Player.Id,
            request.Player.FullName,
            request.Player.UserProfile?.ProfilePictureUrl
        );

        return new MatchJoinRequestResponse(
            request.Id,
            (JoinRequestStatusDto)request.Status,
            playerSummary,
            matchSummary,
            IsMine: request.PlayerId == userId,
            request.RequestedAt
        );
    }

    public async Task<Result<MatchJoinRequestResponse>> RequestToJoinAsync(
        string userId, string matchId, JoinMatchRequest request, CancellationToken ct = default)
    {
        try
        {
            var match = await _context.FriendlyMatches
                .Include(m => m.Organizer)
                .Include(m => m.JoinRequests)
                .FirstOrDefaultAsync(m => m.Id == matchId && !m.IsDeleted, ct);

            if (match is null)
                return Result.Failure<MatchJoinRequestResponse>(MatchErrors.NotFound);

            if (match.Status != MatchStatus.Open)
                return Result.Failure<MatchJoinRequestResponse>(MatchErrors.NotOpen);

            if (match.OrganizerId == userId)
                return Result.Failure<MatchJoinRequestResponse>(MatchErrors.OrganizerCannotJoin);

            if (match.JoinRequests.Any(r => r.PlayerId == userId))
                return Result.Failure<MatchJoinRequestResponse>(MatchErrors.AlreadyRequested);

            var acceptedCount = match.JoinRequests.Count(r => r.Status == JoinRequestStatus.Accepted);
            if (acceptedCount >= match.RequiredPlayers)
                return Result.Failure<MatchJoinRequestResponse>(MatchErrors.MatchFull);

            var player = await _context.Users
                .Include(u => u.UserProfile)
                .FirstOrDefaultAsync(u => u.Id == userId, ct);

            if (player is null)
                return Result.Failure<MatchJoinRequestResponse>(UserErrors.NotFound);

            var joinRequest = new MatchJoinRequest
            {
                FriendlyMatchId = matchId,
                FriendlyMatch = match,
                PlayerId = userId,
                Player = player,
                Status = JoinRequestStatus.Pending,
                RequestedAt = DateTime.UtcNow
            };

            await _context.MatchJoinRequests.AddAsync(joinRequest, ct);
            await _context.SaveChangesAsync(ct);

            // Re-load with nested references (Court, Club) for full mapping response
            var loadedRequest = await _context.MatchJoinRequests
                .Include(r => r.Player)
                .ThenInclude(u => u.UserProfile)
                .Include(r => r.FriendlyMatch)
                .ThenInclude(m => m.Court)
                .ThenInclude(c => c.Club)
                .FirstAsync(r => r.Id == joinRequest.Id, ct);

            // Send notification to Organizer
            await _notificationService.SendNotificationAsync(
                match.OrganizerId,
                Sportiva.Entities.NotificationType.MatchJoinRequestReceived,
                "New Join Request",
                $"{player.FullName} wants to join your friendly match on {match.Date}.",
                actorId: userId,
                entityType: "FriendlyMatch",
                entityId: matchId,
                priority: Sportiva.Entities.NotificationPriority.Normal,
                ct: ct
            );

            var response = MapToResponse(loadedRequest, userId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating join request for user {UserId} to match {MatchId}", userId, matchId);
            return Result.Failure<MatchJoinRequestResponse>(
                new Error("Matches.Error", "An error occurred while creating join request", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result<MatchJoinRequestResponse>> GetJoinRequestAsync(
        string userId, string matchId, string requestId, CancellationToken ct = default)
    {
        try
        {
            var request = await _context.MatchJoinRequests
                .Include(r => r.Player)
                .ThenInclude(u => u.UserProfile)
                .Include(r => r.FriendlyMatch)
                .ThenInclude(m => m.Court)
                .ThenInclude(c => c.Club)
                .FirstOrDefaultAsync(r => r.Id == requestId && r.FriendlyMatchId == matchId, ct);

            if (request is null)
                return Result.Failure<MatchJoinRequestResponse>(MatchErrors.JoinRequestNotFound);

            if (request.PlayerId != userId && request.FriendlyMatch.OrganizerId != userId)
                return Result.Failure<MatchJoinRequestResponse>(MatchErrors.Unauthorized);

            var response = MapToResponse(request, userId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching join request {RequestId}", requestId);
            return Result.Failure<MatchJoinRequestResponse>(
                new Error("Matches.Error", "An error occurred while retrieving join request", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result<PaginatedList<MatchJoinRequestResponse>>> GetMatchJoinRequestsAsync(
        string userId, string matchId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var match = await _context.FriendlyMatches.FirstOrDefaultAsync(m => m.Id == matchId && !m.IsDeleted, ct);
            if (match is null)
                return Result.Failure<PaginatedList<MatchJoinRequestResponse>>(MatchErrors.NotFound);

            if (match.OrganizerId != userId)
                return Result.Failure<PaginatedList<MatchJoinRequestResponse>>(MatchErrors.Unauthorized);

            var baseQuery = _context.MatchJoinRequests
                .Include(r => r.Player)
                .ThenInclude(u => u.UserProfile)
                .Include(r => r.FriendlyMatch)
                .ThenInclude(m => m.Court)
                .ThenInclude(c => c.Club)
                .Where(r => r.FriendlyMatchId == matchId);

            var paginated = await PaginatedList<MatchJoinRequest>.CreateAsync(
                baseQuery.OrderByDescending(r => r.RequestedAt), filters.PageNumber, filters.PageSize, ct);

            var mappedList = paginated.Select(r => MapToResponse(r, userId));

            return Result.Success(mappedList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching join requests for match {MatchId} as organizer {UserId}", matchId, userId);
            return Result.Failure<PaginatedList<MatchJoinRequestResponse>>(
                new Error("Matches.Error", "An error occurred while retrieving requests", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result<PaginatedList<MatchJoinRequestResponse>>> GetMyJoinRequestsAsync(
        string userId, RequestFilters filters, JoinRequestStatus? status = null, CancellationToken ct = default)
    {
        try
        {
            var baseQuery = _context.MatchJoinRequests
                .Include(r => r.Player)
                .ThenInclude(u => u.UserProfile)
                .Include(r => r.FriendlyMatch)
                .ThenInclude(m => m.Court)
                .ThenInclude(c => c.Club)
                .Where(r => r.PlayerId == userId);

            if (status.HasValue)
                baseQuery = baseQuery.Where(r => r.Status == status.Value);

            var paginated = await PaginatedList<MatchJoinRequest>.CreateAsync(
                baseQuery.OrderByDescending(r => r.RequestedAt), filters.PageNumber, filters.PageSize, ct);

            var mappedList = paginated.Select(r => MapToResponse(r, userId));

            return Result.Success(mappedList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching join requests sent by user {UserId}", userId);
            return Result.Failure<PaginatedList<MatchJoinRequestResponse>>(
                new Error("Matches.Error", "An error occurred while retrieving requests history", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result> AcceptJoinRequestAsync(
        string userId, string matchId, string requestId, CancellationToken ct = default)
    {
        try
        {
            var match = await _context.FriendlyMatches
                .Include(m => m.JoinRequests)
                .FirstOrDefaultAsync(m => m.Id == matchId && !m.IsDeleted, ct);

            if (match is null)
                return Result.Failure(MatchErrors.NotFound);

            if (match.OrganizerId != userId)
                return Result.Failure(MatchErrors.Unauthorized);

            var request = match.JoinRequests.FirstOrDefault(r => r.Id == requestId);
            if (request is null)
                return Result.Failure(MatchErrors.JoinRequestNotFound);

            if (request.Status != JoinRequestStatus.Pending)
                return Result.Failure(MatchErrors.NotPending);

            var acceptedCount = match.JoinRequests.Count(r => r.Status == JoinRequestStatus.Accepted);
            if (acceptedCount >= match.RequiredPlayers)
                return Result.Failure(MatchErrors.MatchFull);

            request.Status = JoinRequestStatus.Accepted;

            // Mark match full if cap reached
            if (acceptedCount + 1 >= match.RequiredPlayers)
            {
                match.Status = MatchStatus.Full;
            }

            await _context.SaveChangesAsync(ct);

            // Send notification to Player
            await _notificationService.SendNotificationAsync(
                request.PlayerId,
                Sportiva.Entities.NotificationType.MatchJoinRequestAccepted,
                "Request Accepted",
                $"Your request to join the friendly match on {match.Date} has been accepted.",
                actorId: userId,
                entityType: "FriendlyMatch",
                entityId: matchId,
                priority: Sportiva.Entities.NotificationPriority.Normal,
                ct: ct
            );

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while accepting join request {RequestId} by organizer {UserId}", requestId, userId);
            return Result.Failure(
                new Error("Matches.Error", "An error occurred while accepting join request", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result> RejectJoinRequestAsync(
        string userId, string matchId, string requestId, CancellationToken ct = default)
    {
        try
        {
            var match = await _context.FriendlyMatches
                .Include(m => m.JoinRequests)
                .FirstOrDefaultAsync(m => m.Id == matchId && !m.IsDeleted, ct);

            if (match is null)
                return Result.Failure(MatchErrors.NotFound);

            if (match.OrganizerId != userId)
                return Result.Failure(MatchErrors.Unauthorized);

            var request = match.JoinRequests.FirstOrDefault(r => r.Id == requestId);
            if (request is null)
                return Result.Failure(MatchErrors.JoinRequestNotFound);

            if (request.Status != JoinRequestStatus.Pending)
                return Result.Failure(MatchErrors.NotPending);

            request.Status = JoinRequestStatus.Rejected;

            await _context.SaveChangesAsync(ct);

            // Send notification to Player
            await _notificationService.SendNotificationAsync(
                request.PlayerId,
                Sportiva.Entities.NotificationType.MatchJoinRequestRejected,
                "Request Rejected",
                $"Your request to join the friendly match on {match.Date} has been rejected.",
                actorId: userId,
                entityType: "FriendlyMatch",
                entityId: matchId,
                priority: Sportiva.Entities.NotificationPriority.Normal,
                ct: ct
            );

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while rejecting join request {RequestId} by organizer {UserId}", requestId, userId);
            return Result.Failure(
                new Error("Matches.Error", "An error occurred while rejecting join request", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result> WithdrawJoinRequestAsync(
        string userId, string matchId, string requestId, CancellationToken ct = default)
    {
        try
        {
            var request = await _context.MatchJoinRequests
                .FirstOrDefaultAsync(r => r.Id == requestId && r.FriendlyMatchId == matchId, ct);

            if (request is null)
                return Result.Failure(MatchErrors.JoinRequestNotFound);

            if (request.PlayerId != userId)
                return Result.Failure(MatchErrors.Unauthorized);

            if (request.Status != JoinRequestStatus.Pending)
                return Result.Failure(new Error("Matches.NotWithdrawalPending", "Only pending requests can be withdrawn", StatusCodes.Status400BadRequest));

            _context.MatchJoinRequests.Remove(request);
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while withdrawing join request {RequestId} by player {UserId}", requestId, userId);
            return Result.Failure(
                new Error("Matches.Error", "An error occurred while withdrawing join request", StatusCodes.Status500InternalServerError));
        }
    }
}
