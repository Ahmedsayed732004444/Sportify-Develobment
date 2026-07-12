// UpdateProfilePhotoRequest.cs
namespace Sportiva.Contracts.Users;

public record UpdateProfilePhotoRequest(
    IFormFile ProfilePicture
);