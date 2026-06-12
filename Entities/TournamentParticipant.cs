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
