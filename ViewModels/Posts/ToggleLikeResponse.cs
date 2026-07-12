namespace Sportiva.Contracts.Posts;

public record ToggleLikeResponse(
    string PostId,
    bool   ILiked,      // true = now liked, false = now unliked
    int    LikesCount
);
