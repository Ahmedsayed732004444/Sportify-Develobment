namespace Sportiva.Contracts.Profile;
public sealed record UpdateProfileResponse(
    string Id,
    string FullName,
    string? Bio,
    string? City,
    string? Country
);
