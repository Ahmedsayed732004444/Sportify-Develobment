namespace Sportiva.Entities;

public class CommentReaction
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    public string CommentId { get; set; } = string.Empty;
    public PostComment Comment { get; set; } = default!;

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = default!;

    public DateTime LikedAt { get; set; } = DateTime.UtcNow;
}