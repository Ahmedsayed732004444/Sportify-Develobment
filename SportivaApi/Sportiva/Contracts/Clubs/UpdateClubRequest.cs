namespace Sportiva.Contracts.Clubs;

public record UpdateClubRequest(
    string? Name,
    IFormFile? Logo,
    string? Governorate,
    string? City,
    string? Address,
    string? PhoneNumber,
    string? Email,
    bool IsActive
);
