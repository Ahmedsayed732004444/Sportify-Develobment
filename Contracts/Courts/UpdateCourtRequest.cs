using Sportiva.Contracts.Shared.Enums;

namespace Sportiva.Contracts.Courts;

public record UpdateCourtRequest(
    string?      Name,
    string?      Description,
    string?      ImageUrl,
    SportTypeDto SportType,
    int          MaxCapacity,
    decimal      PricePerHour,
    bool         IsActive
);
