// UpdateProfileInfoRequest.cs
using Sportiva.Contracts.Shared.Enums;

namespace Sportiva.Contracts.Users;

public record UpdateProfileInfoRequest(
    string? FirstName,
    string? LastName,
    string? Bio,
    string? City,
    string? Country,
    SportTypeDto? PreferredSport,
    string? PreferredCity
);