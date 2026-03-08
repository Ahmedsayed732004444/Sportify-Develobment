namespace Sportiva.Entities;

public class MembershipUpgrade
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = default!;
    public RequestStatus Status { get; set; } = RequestStatus.Pending;
    public string? Note { get; set; }
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
}
