using Sportiva.Contracts.Common;
using Sportiva.Contracts.Matches;
using Sportiva.Contracts.Shared.Enums;
using Sportiva.Contracts.Shared.Summaries;
using Sportiva.Abstractions;

namespace Sportiva.Services;

public class FriendlyMatchService(
    ApplicationDbContext context,
    ILogger<FriendlyMatchService> logger) : IFriendlyMatchService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<FriendlyMatchService> _logger = logger;

    private static FriendlyMatchResponse MapToResponse(FriendlyMatch match, string? currentUserId)
    {
        var acceptedRequests = match.JoinRequests
            .Where(r => r.Status == JoinRequestStatus.Accepted)
            .ToList();

        var acceptedPlayersCount = acceptedRequests.Count;
        var slotsRemaining = Math.Max(0, match.RequiredPlayers - acceptedPlayersCount);

        var isOwner = match.OrganizerId == currentUserId;
        var isParticipating = acceptedRequests.Any(r => r.PlayerId == currentUserId);
        var isApplied = match.JoinRequests.Any(r => r.PlayerId == currentUserId && r.Status == JoinRequestStatus.Pending);

        var canJoin = !isOwner && !isParticipating && !isApplied && match.Status == MatchStatus.Open && slotsRemaining > 0;

        var participantsPreview = acceptedRequests
            .OrderBy(r => r.RequestedAt)
            .Take(5)
            .Select(r => new ParticipantSummary(
                r.PlayerId,
                r.Player.FullName,
                r.Player.UserProfile?.ProfilePictureUrl,
                r.RequestedAt
            ))
            .ToList();

        var courtSummary = new CourtSummary(
            match.Court.Id,
            match.Court.Name,
            match.Court.ImageUrl,
            (SportTypeDto)match.Court.SportType,
            match.Court.PricePerHour,
            new ClubSummary(match.Court.Club.Id, match.Court.Club.Name, match.Court.Club.LogoUrl, match.Court.Club.City, match.Court.Club.Governorate)
        );

        var organizerSummary = new UserSummary(
            match.Organizer.Id,
            match.Organizer.FullName,
            match.Organizer.UserProfile?.ProfilePictureUrl
        );

        return new FriendlyMatchResponse(
            match.Id,
            match.Date,
            match.StartTime,
            match.EndTime,
            (SportTypeDto)match.SportType,
            match.RequiredPlayers,
            acceptedPlayersCount,
            slotsRemaining,
            (MatchStatusDto)match.Status,
            match.Note,
            courtSummary,
            organizerSummary,
            isOwner,
            isParticipating,
            isApplied,
            canJoin,
            participantsPreview,
            match.CreatedAt
        );
    }

    public async Task<Result<PaginatedList<FriendlyMatchResponse>>> GetMatchesAsync(
        string? currentUserId, RequestFilters filters,
        SportType? sport = null, DateOnly? date = null, string? city = null,
        CancellationToken ct = default)
    {
        try
        {
            var baseQuery = _context.FriendlyMatches
                .Include(m => m.Organizer)
                .ThenInclude(u => u.UserProfile)
                .Include(m => m.Court)
                .ThenInclude(c => c.Club)
                .Include(m => m.JoinRequests)
                .ThenInclude(r => r.Player)
                .ThenInclude(u => u.UserProfile)
                .Where(m => !m.IsDeleted);

            if (sport.HasValue)
                baseQuery = baseQuery.Where(m => m.SportType == sport.Value);

            if (date.HasValue)
                baseQuery = baseQuery.Where(m => m.Date == date.Value);

            if (!string.IsNullOrWhiteSpace(city))
                baseQuery = baseQuery.Where(m => m.Court.Club.City == city);

            var paginatedMatches = await PaginatedList<FriendlyMatch>.CreateAsync(
                baseQuery.OrderByDescending(m => m.CreatedAt), filters.PageNumber, filters.PageSize, ct);

            var mappedList = paginatedMatches.Select(m => MapToResponse(m, currentUserId));

            return Result.Success(mappedList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving friendly matches");
            return Result.Failure<PaginatedList<FriendlyMatchResponse>>(
                new Error("Matches.Error", "An error occurred while retrieving matches", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result<FriendlyMatchResponse>> GetMatchAsync(
        string matchId, string? currentUserId = null, CancellationToken ct = default)
    {
        try
        {
            var match = await _context.FriendlyMatches
                .Include(m => m.Organizer)
                .ThenInclude(u => u.UserProfile)
                .Include(m => m.Court)
                .ThenInclude(c => c.Club)
                .Include(m => m.JoinRequests)
                .ThenInclude(r => r.Player)
                .ThenInclude(u => u.UserProfile)
                .FirstOrDefaultAsync(m => m.Id == matchId && !m.IsDeleted, ct);

            if (match is null)
                return Result.Failure<FriendlyMatchResponse>(MatchErrors.NotFound);

            var response = MapToResponse(match, currentUserId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving friendly match {MatchId}", matchId);
            return Result.Failure<FriendlyMatchResponse>(
                new Error("Matches.Error", "An error occurred while retrieving the match", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result<FriendlyMatchResponse>> CreateMatchAsync(
        string userId, CreateFriendlyMatchRequest request, CancellationToken ct = default)
    {
        try
        {
            var court = await _context.Courts
                .Include(c => c.Club)
                .FirstOrDefaultAsync(c => c.Id == request.CourtId && !c.IsDeleted, ct);

            if (court is null)
                return Result.Failure<FriendlyMatchResponse>(CourtErrors.CourtNotFound);

            if (!court.IsActive)
                return Result.Failure<FriendlyMatchResponse>(BookingErrors.CourtNotActive);

            var organizer = await _context.Users
                .Include(u => u.UserProfile)
                .FirstOrDefaultAsync(u => u.Id == userId, ct);

            if (organizer is null)
                return Result.Failure<FriendlyMatchResponse>(UserErrors.NotFound);

            var match = new FriendlyMatch
            {
                OrganizerId = userId,
                Organizer = organizer,
                CourtId = request.CourtId,
                Court = court,
                Date = request.Date,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                SportType = (SportType)request.SportType,
                RequiredPlayers = request.RequiredPlayers,
                Status = MatchStatus.Open,
                Note = request.Note,
                CreatedAt = DateTime.UtcNow
            };

            await _context.FriendlyMatches.AddAsync(match, ct);
            await _context.SaveChangesAsync(ct);

            var response = MapToResponse(match, userId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating friendly match for organizer {UserId}", userId);
            return Result.Failure<FriendlyMatchResponse>(
                new Error("Matches.Error", "An error occurred while creating the friendly match", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result<FriendlyMatchResponse>> UpdateMatchAsync(
        string userId, string matchId, CreateFriendlyMatchRequest request, CancellationToken ct = default)
    {
        try
        {
            var match = await _context.FriendlyMatches
                .Include(m => m.Organizer)
                .ThenInclude(u => u.UserProfile)
                .Include(m => m.Court)
                .ThenInclude(c => c.Club)
                .Include(m => m.JoinRequests)
                .ThenInclude(r => r.Player)
                .ThenInclude(u => u.UserProfile)
                .FirstOrDefaultAsync(m => m.Id == matchId && !m.IsDeleted, ct);

            if (match is null)
                return Result.Failure<FriendlyMatchResponse>(MatchErrors.NotFound);

            if (match.OrganizerId != userId)
                return Result.Failure<FriendlyMatchResponse>(MatchErrors.Unauthorized);

            var court = await _context.Courts
                .Include(c => c.Club)
                .FirstOrDefaultAsync(c => c.Id == request.CourtId && !c.IsDeleted, ct);

            if (court is null)
                return Result.Failure<FriendlyMatchResponse>(CourtErrors.CourtNotFound);

            match.CourtId = request.CourtId;
            match.Court = court;
            match.Date = request.Date;
            match.StartTime = request.StartTime;
            match.EndTime = request.EndTime;
            match.SportType = (SportType)request.SportType;
            match.RequiredPlayers = request.RequiredPlayers;
            match.Note = request.Note;

            // Update status if players count changes
            var acceptedCount = match.JoinRequests.Count(r => r.Status == JoinRequestStatus.Accepted);
            match.Status = acceptedCount >= request.RequiredPlayers ? MatchStatus.Full : MatchStatus.Open;

            await _context.SaveChangesAsync(ct);

            var response = MapToResponse(match, userId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating friendly match {MatchId} for user {UserId}", matchId, userId);
            return Result.Failure<FriendlyMatchResponse>(
                new Error("Matches.Error", "An error occurred while updating the friendly match", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result> CancelMatchAsync(
        string userId, string matchId, CancellationToken ct = default)
    {
        try
        {
            var match = await _context.FriendlyMatches
                .FirstOrDefaultAsync(m => m.Id == matchId && !m.IsDeleted, ct);

            if (match is null)
                return Result.Failure(MatchErrors.NotFound);

            if (match.OrganizerId != userId)
                return Result.Failure(MatchErrors.Unauthorized);

            match.Status = MatchStatus.Cancelled;
            match.IsDeleted = true;

            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while cancelling friendly match {MatchId} for user {UserId}", matchId, userId);
            return Result.Failure(
                new Error("Matches.Error", "An error occurred while cancelling the friendly match", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result<IReadOnlyList<ParticipantSummary>>> GetParticipantsAsync(
        string matchId, CancellationToken ct = default)
    {
        try
        {
            var matchExists = await _context.FriendlyMatches.AnyAsync(m => m.Id == matchId && !m.IsDeleted, ct);
            if (!matchExists)
                return Result.Failure<IReadOnlyList<ParticipantSummary>>(MatchErrors.NotFound);

            var participants = await _context.MatchJoinRequests
                .Include(r => r.Player)
                .ThenInclude(u => u.UserProfile)
                .Where(r => r.FriendlyMatchId == matchId && r.Status == JoinRequestStatus.Accepted)
                .OrderBy(r => r.RequestedAt)
                .Select(r => new ParticipantSummary(
                    r.PlayerId,
                    r.Player.FullName,
                    r.Player.UserProfile.ProfilePictureUrl,
                    r.RequestedAt
                ))
                .ToListAsync(ct);

            return Result.Success<IReadOnlyList<ParticipantSummary>>(participants);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching participants for friendly match {MatchId}", matchId);
            return Result.Failure<IReadOnlyList<ParticipantSummary>>(
                new Error("Matches.Error", "An error occurred while retrieving participants", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result> LeaveMatchAsync(
        string userId, string matchId, CancellationToken ct = default)
    {
        try
        {
            var match = await _context.FriendlyMatches
                .Include(m => m.JoinRequests)
                .FirstOrDefaultAsync(m => m.Id == matchId && !m.IsDeleted, ct);

            if (match is null)
                return Result.Failure(MatchErrors.NotFound);

            var acceptedRequest = match.JoinRequests
                .FirstOrDefault(r => r.PlayerId == userId && r.Status == JoinRequestStatus.Accepted);

            if (acceptedRequest is null)
                return Result.Failure(new Error("Matches.NotParticipant", "You are not an accepted participant of this match", StatusCodes.Status400BadRequest));

            _context.MatchJoinRequests.Remove(acceptedRequest);

            // Set status to open if it was full
            if (match.Status == MatchStatus.Full)
            {
                match.Status = MatchStatus.Open;
            }

            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while user {UserId} tried to leave friendly match {MatchId}", userId, matchId);
            return Result.Failure(
                new Error("Matches.Error", "An error occurred while leaving the friendly match", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result<PaginatedList<FriendlyMatchResponse>>> GetMyMatchesAsync(
        string userId, RequestFilters filters, string? role = null, CancellationToken ct = default)
    {
        try
        {
            var baseQuery = _context.FriendlyMatches
                .Include(m => m.Organizer)
                .ThenInclude(u => u.UserProfile)
                .Include(m => m.Court)
                .ThenInclude(c => c.Club)
                .Include(m => m.JoinRequests)
                .ThenInclude(r => r.Player)
                .ThenInclude(u => u.UserProfile)
                .Where(m => !m.IsDeleted);

            if (role == "organizer")
            {
                baseQuery = baseQuery.Where(m => m.OrganizerId == userId);
            }
            else if (role == "player")
            {
                baseQuery = baseQuery.Where(m => m.JoinRequests.Any(r => r.PlayerId == userId && r.Status == JoinRequestStatus.Accepted));
            }
            else
            {
                baseQuery = baseQuery.Where(m => m.OrganizerId == userId || m.JoinRequests.Any(r => r.PlayerId == userId && r.Status == JoinRequestStatus.Accepted));
            }

            var paginatedMatches = await PaginatedList<FriendlyMatch>.CreateAsync(
                baseQuery.OrderByDescending(m => m.CreatedAt), filters.PageNumber, filters.PageSize, ct);

            var mappedList = paginatedMatches.Select(m => MapToResponse(m, userId));

            return Result.Success(mappedList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching organized/joined matches for user {UserId}", userId);
            return Result.Failure<PaginatedList<FriendlyMatchResponse>>(
                new Error("Matches.Error", "An error occurred while retrieving matches history", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result<PaginatedList<FriendlyMatchResponse>>> GetCourtMatchesAsync(
        string courtId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var baseQuery = _context.FriendlyMatches
                .Include(m => m.Organizer)
                .ThenInclude(u => u.UserProfile)
                .Include(m => m.Court)
                .ThenInclude(c => c.Club)
                .Include(m => m.JoinRequests)
                .ThenInclude(r => r.Player)
                .ThenInclude(u => u.UserProfile)
                .Where(m => m.CourtId == courtId && !m.IsDeleted);

            var paginatedMatches = await PaginatedList<FriendlyMatch>.CreateAsync(
                baseQuery.OrderByDescending(m => m.CreatedAt), filters.PageNumber, filters.PageSize, ct);

            var mappedList = paginatedMatches.Select(m => MapToResponse(m, currentUserId: null));

            return Result.Success(mappedList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching matches for court {CourtId}", courtId);
            return Result.Failure<PaginatedList<FriendlyMatchResponse>>(
                new Error("Matches.Error", "An error occurred while retrieving matches", StatusCodes.Status500InternalServerError));
        }
    }
}
