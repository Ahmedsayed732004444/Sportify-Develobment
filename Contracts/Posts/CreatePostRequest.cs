namespace Sportiva.Contracts.Posts;

public record CreatePostRequest(
    string Content,
    IFormFile? File
);
