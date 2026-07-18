namespace Sportiva.Entities;

public class Club
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string? Name { get; set; }
    public string? LogoUrl { get; set; }
    public string? Governorate { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // ✅ الإضافة المطلوبة

    public string OwnerId { get; set; } = string.Empty;
    public ApplicationUser Owner { get; set; } = default!;

    public ICollection<Court> Courts { get; set; } = [];
    public ICollection<ClubSubscription> Subscriptions { get; set; } = [];
}