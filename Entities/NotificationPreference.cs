namespace Sportiva.Entities;

public class NotificationPreference
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = default!;

    public NotificationType Type { get; set; }

    public bool InAppEnabled { get; set; } = true;
    public bool EmailEnabled { get; set; } = false;
}