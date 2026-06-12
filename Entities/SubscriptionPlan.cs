
namespace Sportiva.Entities;

public class SubscriptionPlan
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string Name { get; set; } = string.Empty;        // Basic, Pro, Enterprise
    public string? Description { get; set; }
    public decimal MonthlyPrice { get; set; }
    public int MaxCourts { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public ICollection<ClubSubscription> ClubSubscriptions { get; set; } = [];
}
