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
