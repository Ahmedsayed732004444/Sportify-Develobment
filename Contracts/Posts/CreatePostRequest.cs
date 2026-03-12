namespace Sportiva.Contracts.Posts;
public sealed record CreatePostRequest(
    string Content,
    IFormFile? File 
);
