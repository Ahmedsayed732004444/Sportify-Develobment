namespace Sportiva.Contracts.Profile;
public sealed record UpdateProfileRequest(
    string? Bio,
    string? City,
    string? Country
);
