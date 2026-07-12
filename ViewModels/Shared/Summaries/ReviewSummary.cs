namespace Sportiva.Contracts.Shared.Summaries;

public record ReviewSummary(
    string   ReviewId,
    int      Rating,
    string?  Comment,
    DateTime CreatedAt
);
