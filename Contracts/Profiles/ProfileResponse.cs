namespace Sportiva.Contracts.Profile;
public sealed record ProfileResponse(
    string Id,
    string FullName,
    string? Bio,
    string? City,
    string? Country,
    string? ProfilePictureUrl,
    string? CoverImageUrl
);
