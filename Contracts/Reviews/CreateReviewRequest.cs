namespace Sportiva.Contracts.Reviews;

public record CreateReviewRequest(
    string  BookingId,
    int     Rating,
    string? Comment
);
