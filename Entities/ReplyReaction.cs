namespace Sportiva.Entities;

public class ReplyReaction
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    public string ReplyId { get; set; } = string.Empty;
    public CommentReply Reply { get; set; } = default!;

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = default!;

    public DateTime LikedAt { get; set; } = DateTime.UtcNow;
}