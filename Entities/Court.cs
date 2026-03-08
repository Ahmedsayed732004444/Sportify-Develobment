namespace Sportiva.Entities;
public class Court
{
   public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string ClubId { get; set; } = string.Empty;
    public Club Club { get; set; }
    public string? Name { get; set; }
    public decimal PricePerHour { get; set; }
    public string? Description { get; set; }
    public SportType SportType { get; set; }= SportType.Football;
    public int MaxCapacity { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; } = false;
    public string? ImageUrl { get; set; }
    public ICollection<TimeSlot> TimeSlots { get; set; }
}
