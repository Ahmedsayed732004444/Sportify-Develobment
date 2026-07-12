namespace Sportiva.Contracts.Shared.Summaries;

public record ParticipantSummary(
    string   UserId,
    string   FullName,
    string?  ProfilePictureUrl,
    DateTime JoinedAt
);
