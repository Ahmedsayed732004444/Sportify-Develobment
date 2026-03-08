namespace Sportiva.Entities;

public class UserProfile
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = default!;
    public string? Bio { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public ICollection<Post> Posts { get; set; } = [];
}
