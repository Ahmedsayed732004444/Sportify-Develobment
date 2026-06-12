namespace Sportiva.Contracts.Shared.Summaries;

public record UserSummary(
    string  UserId,
    string  FullName,
    string? ProfilePictureUrl
);
