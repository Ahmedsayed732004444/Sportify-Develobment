namespace Sportiva.Entities;

public class CommentReply
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    public string CommentId { get; set; } = string.Empty;
    public PostComment Comment { get; set; } = default!;

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = default!;

    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;

    public ICollection<ReplyReaction> Reactions { get; set; } = [];
}