using Sportiva.Contracts.Shared.Enums;

namespace Sportiva.Contracts.Courts;

public record CreateCourtRequest(
    string? Name,
    string? Description,
    IFormFile? Image,
    SportTypeDto SportType,
    int MaxCapacity,
    decimal PricePerHour
);