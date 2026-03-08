
namespace Sportiva.Entities;
public class Booking
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    public string CourtId { get; set; } = string.Empty;
    public Court Court { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; }

    public string TimeSlotId { get; set; } = string.Empty;
    public TimeSlot TimeSlot { get; set; }

    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    public decimal Price { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
}
