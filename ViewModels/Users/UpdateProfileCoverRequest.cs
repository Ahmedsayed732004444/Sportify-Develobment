// UpdateProfileCoverRequest.cs
namespace Sportiva.Contracts.Users;

public record UpdateProfileCoverRequest(
    IFormFile CoverImage
);