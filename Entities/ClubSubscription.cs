namespace Sportiva.Entities;

public class ClubSubscription
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    public string ClubId { get; set; } = string.Empty;
    public Club Club { get; set; } = default!;

    public string PlanId { get; set; } = string.Empty;
    public SubscriptionPlan Plan { get; set; } = default!;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }                   // StartDate + 30 يوم

    public bool IsActive => DateTime.UtcNow <= EndDate;     // computed - مش محتاج تخزنه

    public ICollection<SubscriptionPayment> Payments { get; set; } = [];
}
