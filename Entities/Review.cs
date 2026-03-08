namespace Sportiva.Entities;

public class Review
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string CourtId { get; set; } = string.Empty;
    public Court Court { get; set; } = default!;
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = default!;
    public string BookingId { get; set; } = string.Empty;
    public Booking Booking { get; set; } = default!;
    public int Rating { get; set; }               // من 1 لـ 5
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
}
