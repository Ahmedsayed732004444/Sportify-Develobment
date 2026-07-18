namespace Sportiva.Entities;

public class TimeSlot
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string CourtId { get; set; } = string.Empty;
    public Court Court { get; set; } = default!;
    public DateOnly Day { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsActive { get; set; } = true;   // ✅ جديد — Owner بيتحكم فيها
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;

    public ICollection<Booking> Bookings { get; set; } = [];
    public bool IsBooked => Bookings.Any(b =>
     !b.IsDeleted &&
     (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending));
    // ده زي ما هو بالظبط، بس تأكد إن Rejected/Cancelled مش داخلين فيه (مش داخلين أصلاً، تمام)
}