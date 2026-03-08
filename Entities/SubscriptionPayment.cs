namespace Sportiva.Entities;
public class SubscriptionPayment
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    public string ClubSubscriptionId { get; set; } = string.Empty;
    public ClubSubscription ClubSubscription { get; set; } = default!;

    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? TransactionId { get; set; }              // من بوابة الدفع
}
