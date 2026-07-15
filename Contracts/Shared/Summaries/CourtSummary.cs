using Sportiva.Contracts.Shared.Enums;

namespace Sportiva.Contracts.Shared.Summaries;

public record CourtSummary(
    string       CourtId,
    string?      Name,
    string?      ImageUrl,
    SportTypeDto SportType,
    decimal      PricePerHour,
    ClubSummary  Club
);
