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
