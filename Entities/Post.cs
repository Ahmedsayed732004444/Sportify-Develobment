namespace Sportiva.Entities;
public class Post
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    public string UserProfileId { get; set; } = string.Empty;
    public UserProfile UserProfile { get; set; } = default!;
    public string Content { get; set; } = string.Empty;
    public string? FileUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public ICollection<PostLike> Likes { get; set; } = [];
}
