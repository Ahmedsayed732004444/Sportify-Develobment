namespace Sportiva.Contracts.Posts;
public sealed record PostAuthorSummary(
    string UserProfileId,
    string FullName,
    string? ProfilePictureUrl,
    string? City
);
