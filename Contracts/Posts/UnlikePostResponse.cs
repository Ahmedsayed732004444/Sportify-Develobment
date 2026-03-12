namespace Sportiva.Contracts.Posts;
public sealed record UnlikePostResponse(
    string PostId,
    int LikesCount
);
