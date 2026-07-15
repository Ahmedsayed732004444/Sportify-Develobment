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
