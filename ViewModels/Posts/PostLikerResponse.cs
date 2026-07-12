namespace Sportiva.Contracts.Posts;

public record PostLikerResponse(
    string   UserId,
    string   FullName,
    string?  ProfilePictureUrl,
    DateTime LikedAt
);
