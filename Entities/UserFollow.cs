namespace Sportiva.Entities;

public class UserFollow
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    public string FollowerId { get; set; } = string.Empty;  // اللي بيعمل follow
    public ApplicationUser Follower { get; set; } = default!;

    public string FollowingId { get; set; } = string.Empty; // اللي اتعمله follow
    public ApplicationUser Following { get; set; } = default!;

    public DateTime FollowedAt { get; set; } = DateTime.UtcNow;
}