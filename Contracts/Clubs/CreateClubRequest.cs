namespace Sportiva.Contracts.Clubs;

public record CreateClubRequest(
    string? Name,
    IFormFile? Logo,
    string? Governorate,
    string? City,
    string? Address,
    string? PhoneNumber,
    string? Email
);
