namespace Sportiva.Entities;
public class TimeSlot
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string CourtId { get; set; } = string.Empty;
    public Court Court { get; set; }
    public DateOnly Day { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsBooked { get; set; } = false;
    public bool IsDeleted { get; set; } = false;
    public ICollection<Booking> Bookings { get; set; } 

}