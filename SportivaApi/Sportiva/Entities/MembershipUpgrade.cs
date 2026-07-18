namespace Sportiva.Entities;

// Entities/MembershipUpgrade.cs
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
    public DateTime? ReviewedAt { get; set; }   // ← جديد، لازم عشان الـ Response محتاجها
}