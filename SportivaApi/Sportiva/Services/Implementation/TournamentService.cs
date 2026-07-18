using Sportiva.Contracts.Common;
using Sportiva.Contracts.Tournaments;
using Sportiva.Contracts.Shared.Enums;
using Sportiva.Contracts.Shared.Summaries;
using Sportiva.Abstractions;
using Sportiva.Enums;

namespace Sportiva.Services;

public class TournamentService(
    ApplicationDbContext context,
    ILogger<TournamentService> logger) : ITournamentService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<TournamentService> _logger = logger;

    private static TournamentResponse MapToResponse(Tournament tournament, string? currentUserId)
    {
        var isOwner = tournament.OrganizerId == currentUserId;
        var iParticipating = tournament.Participants.Any(p => p.UserId == currentUserId);
        var canJoin = !isOwner && !iParticipating && tournament.Participants.Count < tournament.MaxParticipants;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var status = TournamentStatusDto.Upcoming;
        if (today > tournament.EndDate)
            status = TournamentStatusDto.Completed;
        else if (today >= tournament.StartDate)
            status = TournamentStatusDto.Ongoing;

        var completedMatchesCount = tournament.Matches.Count(m => !string.IsNullOrEmpty(m.WinnerId));

        var organizerSummary = new UserSummary(
            tournament.Organizer.Id,
            tournament.Organizer.FullName,
            tournament.Organizer.UserProfile?.ProfilePictureUrl
        );

        return new TournamentResponse(
            tournament.Id,
            tournament.Name,
            tournament.Description,
            (SportTypeDto)tournament.SportType,
            status,
            tournament.StartDate,
            tournament.EndDate,
            tournament.MaxParticipants,
            organizerSummary,
            isOwner,
            iParticipating,
            canJoin,
            ParticipantsCount: tournament.Participants.Count,
            MatchesCount: tournament.Matches.Count,
            CompletedMatchesCount: completedMatchesCount,
            tournament.CreatedAt
        );
    }

    private static TournamentMatchResponse MapToMatchResponse(TournamentMatch match)
    {
        var player1Summary = new UserSummary(
            match.Player1.Id,
            match.Player1.FullName,
            match.Player1.UserProfile?.ProfilePictureUrl
        );

        var player2Summary = new UserSummary(
            match.Player2.Id,
            match.Player2.FullName,
            match.Player2.UserProfile?.ProfilePictureUrl
        );

        UserSummary? winnerSummary = null;
        if (match.Winner != null)
        {
            winnerSummary = new UserSummary(
                match.Winner.Id,
                match.Winner.FullName,
                match.Winner.UserProfile?.ProfilePictureUrl
            );
        }

        return new TournamentMatchResponse(
            match.Id,
            match.TournamentId,
            Round: null,
            MatchNumber: null,
            player1Summary,
            player2Summary,
            winnerSummary,
            IsDecided: !string.IsNullOrEmpty(match.WinnerId),
            match.MatchDate,
            match.StartTime
        );
    }

    public async Task<Result<PaginatedList<TournamentResponse>>> GetTournamentsAsync(
        string? currentUserId, RequestFilters filters,
        SportType? sport = null, TournamentStatus? status = null, string? city = null,
        CancellationToken ct = default)
    {
        try
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var baseQuery = _context.Tournaments
                .Include(t => t.Organizer)
                .ThenInclude(u => u.UserProfile)
                .Include(t => t.Participants)
                .Include(t => t.Matches)
                .Where(t => !t.IsDeleted);

            if (sport.HasValue)
                baseQuery = baseQuery.Where(t => t.SportType == sport.Value);

            if (status.HasValue)
            {
                baseQuery = status.Value switch
                {
                    TournamentStatus.Upcoming => baseQuery.Where(t => t.StartDate > today),
                    TournamentStatus.Ongoing => baseQuery.Where(t => t.StartDate <= today && t.EndDate >= today),
                    TournamentStatus.Completed => baseQuery.Where(t => t.EndDate < today),
                    _ => baseQuery
                };
            }

            var paginated = await PaginatedList<Tournament>.CreateAsync(
                baseQuery.OrderByDescending(t => t.CreatedAt), filters.PageNumber, filters.PageSize, ct);

            var mappedList = paginated.Select(t => MapToResponse(t, currentUserId));
            return Result.Success(mappedList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving tournaments list");
            return Result.Failure<PaginatedList<TournamentResponse>>(
                new Error("Tournaments.Error", "An error occurred while retrieving tournaments", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result<TournamentResponse>> GetTournamentAsync(
        string tournamentId, string? currentUserId = null, CancellationToken ct = default)
    {
        try
        {
            var tournament = await _context.Tournaments
                .Include(t => t.Organizer)
                .ThenInclude(u => u.UserProfile)
                .Include(t => t.Participants)
                .Include(t => t.Matches)
                .FirstOrDefaultAsync(t => t.Id == tournamentId && !t.IsDeleted, ct);

            if (tournament is null)
                return Result.Failure<TournamentResponse>(TournamentErrors.NotFound);

            var response = MapToResponse(tournament, currentUserId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving tournament {TournamentId}", tournamentId);
            return Result.Failure<TournamentResponse>(
                new Error("Tournaments.Error", "An error occurred while retrieving the tournament", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result<TournamentResponse>> CreateTournamentAsync(
        string userId, CreateTournamentRequest request, CancellationToken ct = default)
    {
        try
        {
            var organizer = await _context.Users
                .Include(u => u.UserProfile)
                .FirstOrDefaultAsync(u => u.Id == userId, ct);

            if (organizer is null)
                return Result.Failure<TournamentResponse>(UserErrors.NotFound);

            // Enforce subscription active tournaments limit
            var club = await _context.Clubs
                .FirstOrDefaultAsync(c => c.OwnerId == userId && !c.IsDeleted, ct);

            if (club != null)
            {
                var now = DateTime.UtcNow;
                var activeSubscription = await _context.ClubSubscriptions
                    .Include(s => s.Plan)
                    .Where(s => s.ClubId == club.Id && !s.IsDeleted &&
                                s.StartDate <= now && s.EndDate >= now)
                    .FirstOrDefaultAsync(ct);

                if (activeSubscription != null)
                {
                    string planName = activeSubscription.Plan.Name.ToLower();
                    var maxActiveTournaments = 10;
                    if (planName.Contains("basic")) maxActiveTournaments = 1;
                    else if (planName.Contains("premium")) maxActiveTournaments = 3;

                    var todayDate = DateOnly.FromDateTime(now);
                    var activeTournamentsCount = await _context.Tournaments
                        .CountAsync(t => t.OrganizerId == userId && !t.IsDeleted && t.EndDate >= todayDate, ct);

                    if (activeTournamentsCount >= maxActiveTournaments)
                    {
                        return Result.Failure<TournamentResponse>(
                            new Error("Tournaments.LimitReached", $"You have reached the maximum number of active tournaments ({maxActiveTournaments}) allowed by your subscription plan.", StatusCodes.Status400BadRequest));
                    }
                }
                else
                {
                    return Result.Failure<TournamentResponse>(
                        new Error("Tournaments.NoSubscription", "An active subscription is required to host tournaments.", StatusCodes.Status400BadRequest));
                }
            }

            var tournament = new Tournament
            {
                Name = request.Name,
                Description = request.Description,
                OrganizerId = userId,
                Organizer = organizer,
                SportType = (SportType)request.SportType,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                MaxParticipants = request.MaxParticipants,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Tournaments.AddAsync(tournament, ct);
            await _context.SaveChangesAsync(ct);

            var response = MapToResponse(tournament, userId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating tournament for organizer {UserId}", userId);
            return Result.Failure<TournamentResponse>(
                new Error("Tournaments.Error", "An error occurred while creating the tournament", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result<TournamentResponse>> UpdateTournamentAsync(
        string userId, string tournamentId, CreateTournamentRequest request, CancellationToken ct = default)
    {
        try
        {
            var tournament = await _context.Tournaments
                .Include(t => t.Organizer)
                .ThenInclude(u => u.UserProfile)
                .Include(t => t.Participants)
                .Include(t => t.Matches)
                .FirstOrDefaultAsync(t => t.Id == tournamentId && !t.IsDeleted, ct);

            if (tournament is null)
                return Result.Failure<TournamentResponse>(TournamentErrors.NotFound);

            if (tournament.OrganizerId != userId)
                return Result.Failure<TournamentResponse>(TournamentErrors.Unauthorized);

            tournament.Name = request.Name;
            tournament.Description = request.Description;
            tournament.SportType = (SportType)request.SportType;
            tournament.StartDate = request.StartDate;
            tournament.EndDate = request.EndDate;
            tournament.MaxParticipants = request.MaxParticipants;

            await _context.SaveChangesAsync(ct);

            var response = MapToResponse(tournament, userId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating tournament {TournamentId} for user {UserId}", tournamentId, userId);
            return Result.Failure<TournamentResponse>(
                new Error("Tournaments.Error", "An error occurred while updating the tournament", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result> CancelTournamentAsync(
        string userId, string tournamentId, CancellationToken ct = default)
    {
        try
        {
            var tournament = await _context.Tournaments
                .FirstOrDefaultAsync(t => t.Id == tournamentId && !t.IsDeleted, ct);

            if (tournament is null)
                return Result.Failure(TournamentErrors.NotFound);

            if (tournament.OrganizerId != userId)
                return Result.Failure(TournamentErrors.Unauthorized);

            tournament.IsDeleted = true;
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while cancelling tournament {TournamentId} for user {UserId}", tournamentId, userId);
            return Result.Failure(
                new Error("Tournaments.Error", "An error occurred while cancelling the tournament", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result> JoinTournamentAsync(
        string userId, string tournamentId, CancellationToken ct = default)
    {
        try
        {
            var tournament = await _context.Tournaments
                .Include(t => t.Participants)
                .FirstOrDefaultAsync(t => t.Id == tournamentId && !t.IsDeleted, ct);

            if (tournament is null)
                return Result.Failure(TournamentErrors.NotFound);

            if (tournament.OrganizerId == userId)
                return Result.Failure(new Error("Tournaments.OrganizerCannotJoin", "As organizer, you cannot join as participant", StatusCodes.Status400BadRequest));

            if (tournament.Participants.Any(p => p.UserId == userId))
                return Result.Failure(TournamentErrors.AlreadyJoined);

            if (tournament.Participants.Count >= tournament.MaxParticipants)
                return Result.Failure(TournamentErrors.TournamentFull);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
            if (user is null)
                return Result.Failure(UserErrors.NotFound);

            var participant = new TournamentParticipant
            {
                TournamentId = tournamentId,
                Tournament = tournament,
                UserId = userId,
                User = user,
                JoinedAt = DateTime.UtcNow
            };

            await _context.TournamentParticipants.AddAsync(participant, ct);
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while user {UserId} tried to join tournament {TournamentId}", userId, tournamentId);
            return Result.Failure(
                new Error("Tournaments.Error", "An error occurred while joining the tournament", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result> LeaveTournamentAsync(
        string userId, string tournamentId, CancellationToken ct = default)
    {
        try
        {
            var participant = await _context.TournamentParticipants
                .FirstOrDefaultAsync(p => p.UserId == userId && p.TournamentId == tournamentId, ct);

            if (participant is null)
                return Result.Failure(TournamentErrors.NotJoined);

            _context.TournamentParticipants.Remove(participant);
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while user {UserId} tried to leave tournament {TournamentId}", userId, tournamentId);
            return Result.Failure(
                new Error("Tournaments.Error", "An error occurred while leaving the tournament", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result<PaginatedList<ParticipantSummary>>> GetTournamentParticipantsAsync(
        string tournamentId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var tournamentExists = await _context.Tournaments.AnyAsync(t => t.Id == tournamentId && !t.IsDeleted, ct);
            if (!tournamentExists)
                return Result.Failure<PaginatedList<ParticipantSummary>>(TournamentErrors.NotFound);

            var baseQuery = _context.TournamentParticipants
                .Include(p => p.User)
                .ThenInclude(u => u.UserProfile)
                .Where(p => p.TournamentId == tournamentId);

            var paginated = await PaginatedList<TournamentParticipant>.CreateAsync(
                baseQuery.OrderBy(p => p.JoinedAt), filters.PageNumber, filters.PageSize, ct);

            var mappedList = paginated.Select(p => new ParticipantSummary(
                p.UserId,
                p.User.FullName,
                p.User.UserProfile?.ProfilePictureUrl,
                p.JoinedAt
            ));

            return Result.Success(mappedList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving tournament participants for {TournamentId}", tournamentId);
            return Result.Failure<PaginatedList<ParticipantSummary>>(
                new Error("Tournaments.Error", "An error occurred while retrieving participants", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result<PaginatedList<TournamentResponse>>> GetMyTournamentsAsync(
        string userId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var baseQuery = _context.Tournaments
                .Include(t => t.Organizer)
                .ThenInclude(u => u.UserProfile)
                .Include(t => t.Participants)
                .Include(t => t.Matches)
                .Where(t => !t.IsDeleted && (t.OrganizerId == userId || t.Participants.Any(p => p.UserId == userId)));

            var paginated = await PaginatedList<Tournament>.CreateAsync(
                baseQuery.OrderByDescending(t => t.CreatedAt), filters.PageNumber, filters.PageSize, ct);

            var mappedList = paginated.Select(t => MapToResponse(t, userId));
            return Result.Success(mappedList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching tournaments history for user {UserId}", userId);
            return Result.Failure<PaginatedList<TournamentResponse>>(
                new Error("Tournaments.Error", "An error occurred while retrieving tournaments history", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result<IReadOnlyList<TournamentMatchResponse>>> GetTournamentMatchesAsync(
        string tournamentId, int? round = null, CancellationToken ct = default)
    {
        try
        {
            var tournamentExists = await _context.Tournaments.AnyAsync(t => t.Id == tournamentId && !t.IsDeleted, ct);
            if (!tournamentExists)
                return Result.Failure<IReadOnlyList<TournamentMatchResponse>>(TournamentErrors.NotFound);

            var matches = await _context.TournamentMatches
                .Include(m => m.Player1)
                .ThenInclude(u => u.UserProfile)
                .Include(m => m.Player2)
                .ThenInclude(u => u.UserProfile)
                .Include(m => m.Winner)
                .ThenInclude(u => u!.UserProfile)
                .Where(m => m.TournamentId == tournamentId)
                .OrderBy(m => m.MatchDate)
                .Select(m => MapToMatchResponse(m))
                .ToListAsync(ct);

            return Result.Success<IReadOnlyList<TournamentMatchResponse>>(matches);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching tournament matches for {TournamentId}", tournamentId);
            return Result.Failure<IReadOnlyList<TournamentMatchResponse>>(
                new Error("Tournaments.Error", "An error occurred while retrieving tournament matches", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result<TournamentMatchResponse>> GetTournamentMatchAsync(
        string tournamentId, string matchId, CancellationToken ct = default)
    {
        try
        {
            var match = await _context.TournamentMatches
                .Include(m => m.Player1)
                .ThenInclude(u => u.UserProfile)
                .Include(m => m.Player2)
                .ThenInclude(u => u.UserProfile)
                .Include(m => m.Winner)
                .ThenInclude(u => u!.UserProfile)
                .FirstOrDefaultAsync(m => m.Id == matchId && m.TournamentId == tournamentId, ct);

            if (match is null)
                return Result.Failure<TournamentMatchResponse>(TournamentErrors.TournamentMatchNotFound);

            var response = MapToMatchResponse(match);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving tournament match {MatchId} of tournament {TournamentId}", matchId, tournamentId);
            return Result.Failure<TournamentMatchResponse>(
                new Error("Tournaments.Error", "An error occurred while retrieving the tournament match", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result<TournamentMatchResponse>> ScheduleTournamentMatchAsync(
        string userId, string tournamentId, CreateTournamentMatchRequest request, CancellationToken ct = default)
    {
        try
        {
            var tournament = await _context.Tournaments
                .Include(t => t.Participants)
                .FirstOrDefaultAsync(t => t.Id == tournamentId && !t.IsDeleted, ct);

            if (tournament is null)
                return Result.Failure<TournamentMatchResponse>(TournamentErrors.NotFound);

            if (tournament.OrganizerId != userId)
                return Result.Failure<TournamentMatchResponse>(TournamentErrors.Unauthorized);

            var p1Registered = tournament.Participants.Any(p => p.UserId == request.Player1Id);
            var p2Registered = tournament.Participants.Any(p => p.UserId == request.Player2Id);

            if (!p1Registered || !p2Registered)
                return Result.Failure<TournamentMatchResponse>(new Error("Tournaments.PlayersNotRegistered", "Both players must be registered participants in this tournament", StatusCodes.Status400BadRequest));

            var p1 = await _context.Users.Include(u => u.UserProfile).FirstAsync(u => u.Id == request.Player1Id, ct);
            var p2 = await _context.Users.Include(u => u.UserProfile).FirstAsync(u => u.Id == request.Player2Id, ct);

            var match = new TournamentMatch
            {
                TournamentId = tournamentId,
                Tournament = tournament,
                Player1Id = request.Player1Id,
                Player1 = p1,
                Player2Id = request.Player2Id,
                Player2 = p2,
                MatchDate = request.MatchDate,
                StartTime = request.StartTime
            };

            await _context.TournamentMatches.AddAsync(match, ct);
            await _context.SaveChangesAsync(ct);

            var response = MapToMatchResponse(match);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while scheduling match of tournament {TournamentId}", tournamentId);
            return Result.Failure<TournamentMatchResponse>(
                new Error("Tournaments.Error", "An error occurred while scheduling the tournament match", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result<TournamentMatchResponse>> UpdateTournamentMatchAsync(
        string userId, string tournamentId, string matchId, CreateTournamentMatchRequest request, CancellationToken ct = default)
    {
        try
        {
            var match = await _context.TournamentMatches
                .Include(m => m.Player1)
                .ThenInclude(u => u.UserProfile)
                .Include(m => m.Player2)
                .ThenInclude(u => u.UserProfile)
                .Include(m => m.Winner)
                .ThenInclude(u => u!.UserProfile)
                .Include(m => m.Tournament)
                .FirstOrDefaultAsync(m => m.Id == matchId && m.TournamentId == tournamentId && !m.Tournament.IsDeleted, ct);

            if (match is null)
                return Result.Failure<TournamentMatchResponse>(TournamentErrors.TournamentMatchNotFound);

            if (match.Tournament.OrganizerId != userId)
                return Result.Failure<TournamentMatchResponse>(TournamentErrors.Unauthorized);

            var p1Registered = await _context.TournamentParticipants.AnyAsync(p => p.TournamentId == tournamentId && p.UserId == request.Player1Id, ct);
            var p2Registered = await _context.TournamentParticipants.AnyAsync(p => p.TournamentId == tournamentId && p.UserId == request.Player2Id, ct);

            if (!p1Registered || !p2Registered)
                return Result.Failure<TournamentMatchResponse>(new Error("Tournaments.PlayersNotRegistered", "Both players must be registered participants in this tournament", StatusCodes.Status400BadRequest));

            var p1 = await _context.Users.Include(u => u.UserProfile).FirstAsync(u => u.Id == request.Player1Id, ct);
            var p2 = await _context.Users.Include(u => u.UserProfile).FirstAsync(u => u.Id == request.Player2Id, ct);

            match.Player1Id = request.Player1Id;
            match.Player1 = p1;
            match.Player2Id = request.Player2Id;
            match.Player2 = p2;
            match.MatchDate = request.MatchDate;
            match.StartTime = request.StartTime;

            await _context.SaveChangesAsync(ct);

            var response = MapToMatchResponse(match);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating match {MatchId} of tournament {TournamentId}", matchId, tournamentId);
            return Result.Failure<TournamentMatchResponse>(
                new Error("Tournaments.Error", "An error occurred while updating the tournament match", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result> CancelTournamentMatchAsync(
        string userId, string tournamentId, string matchId, CancellationToken ct = default)
    {
        try
        {
            var match = await _context.TournamentMatches
                .Include(m => m.Tournament)
                .FirstOrDefaultAsync(m => m.Id == matchId && m.TournamentId == tournamentId, ct);

            if (match is null)
                return Result.Failure(TournamentErrors.TournamentMatchNotFound);

            if (match.Tournament.OrganizerId != userId)
                return Result.Failure(TournamentErrors.Unauthorized);

            _context.TournamentMatches.Remove(match);
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while cancelling match {MatchId} of tournament {TournamentId}", matchId, tournamentId);
            return Result.Failure(
                new Error("Tournaments.Error", "An error occurred while cancelling the tournament match", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<Result<TournamentMatchResponse>> SetMatchWinnerAsync(
        string userId, string tournamentId, string matchId, SetTournamentMatchWinnerRequest request, CancellationToken ct = default)
    {
        try
        {
            var match = await _context.TournamentMatches
                .Include(m => m.Player1)
                .ThenInclude(u => u.UserProfile)
                .Include(m => m.Player2)
                .ThenInclude(u => u.UserProfile)
                .Include(m => m.Tournament)
                .FirstOrDefaultAsync(m => m.Id == matchId && m.TournamentId == tournamentId, ct);

            if (match is null)
                return Result.Failure<TournamentMatchResponse>(TournamentErrors.TournamentMatchNotFound);

            if (match.Tournament.OrganizerId != userId)
                return Result.Failure<TournamentMatchResponse>(TournamentErrors.Unauthorized);

            if (match.Player1Id != request.WinnerId && match.Player2Id != request.WinnerId)
                return Result.Failure<TournamentMatchResponse>(TournamentErrors.InvalidWinner);

            var winner = await _context.Users.Include(u => u.UserProfile).FirstAsync(u => u.Id == request.WinnerId, ct);

            match.WinnerId = request.WinnerId;
            match.Winner = winner;

            await _context.SaveChangesAsync(ct);

            var response = MapToMatchResponse(match);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deciding winner of match {MatchId} of tournament {TournamentId}", matchId, tournamentId);
            return Result.Failure<TournamentMatchResponse>(
                new Error("Tournaments.Error", "An error occurred while recording the match winner", StatusCodes.Status500InternalServerError));
        }
    }
}
