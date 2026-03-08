namespace Sportiva.Entities;

public class PostLike
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    public string PostId { get; set; } = string.Empty;
    public Post Post { get; set; } = default!;

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = default!;

    public DateTime LikedAt { get; set; } = DateTime.UtcNow;
}