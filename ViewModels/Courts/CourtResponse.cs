using Sportiva.Contracts.Shared.Enums;
using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Courts;

public record CourtResponse(
    string       CourtId,
    string?      Name,
    string?      Description,
    string?      ImageUrl,
    SportTypeDto SportType,
    int          MaxCapacity,
    decimal      PricePerHour,
    bool         IsActive,

    ClubSummary Club,

    bool CanBook,
    bool CanManage,

    int    ReviewsCount,
    double AverageRating,

    DateTime CreatedAt
);
