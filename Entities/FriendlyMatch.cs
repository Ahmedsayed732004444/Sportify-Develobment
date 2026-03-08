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
    public SportType SportType { get; set; }=SportType.Football;
    public int RequiredPlayers { get; set; }   
    public MatchStatus Status { get; set; } = MatchStatus.Open;
    public string? Note { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<MatchJoinRequest> JoinRequests { get; set; } = [];
}
