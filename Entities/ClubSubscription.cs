using Sportiva.Enums;

namespace Sportiva.Entities;

public class ClubSubscription
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = default!;

    public string ClubId { get; set; } = string.Empty;
    public Club Club { get; set; } = default!;

    public string PlanId { get; set; } = string.Empty;
    public SubscriptionPlan Plan { get; set; } = default!;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Price { get; set; } // Total price paid for this subscription

    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.PendingPayment;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CancelledAt { get; set; }
    public decimal? RefundAmount { get; set; } // Amount refunded upon cancellation

    public bool IsDeleted { get; set; } = false;

    public ICollection<SubscriptionPayment> Payments { get; set; } = [];
}