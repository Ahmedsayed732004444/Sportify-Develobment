namespace Sportiva.Contracts.Clubs;

public record CreateClubRequest(
    string? Name,
    string? LogoUrl,
    string? Governorate,
    string? City,
    string? Address,
    string? PhoneNumber,
    string? Email
);
