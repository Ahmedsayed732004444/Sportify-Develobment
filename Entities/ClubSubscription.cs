namespace Sportiva.Entities;

public class ClubSubscription
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    public string ClubId { get; set; } = string.Empty;
    public Club Club { get; set; } = default!;

    public string PlanId { get; set; } = string.Empty;
    public SubscriptionPlan Plan { get; set; } = default!;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsDeleted { get; set; } = false;
    public bool IsActive => !IsDeleted && DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate;

    public ICollection<SubscriptionPayment> Payments { get; set; } = [];
}