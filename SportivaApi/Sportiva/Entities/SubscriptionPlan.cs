namespace Sportiva.Entities;

public class SubscriptionPlan
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int MaxCourts { get; set; }
    public int DurationInDays { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ClubSubscription> ClubSubscriptions { get; set; } = [];
}