namespace Sportiva.Contracts.Shared.Summaries;

public record ClubSummary(
    string  ClubId,
    string? Name,
    string? LogoUrl,
    string? City,
    string? Governorate
);
