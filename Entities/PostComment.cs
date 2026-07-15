namespace Sportiva.Entities;

public class PostComment
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    public string PostId { get; set; } = string.Empty;
    public Post Post { get; set; } = default!;

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = default!;

    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;

    public ICollection<CommentReaction> Reactions { get; set; } = [];
    public ICollection<CommentReply> Replies { get; set; } = [];
}