namespace Sportiva.Contracts.Authentication;

public record ConfirmEmailRequest(
    string UserId,
    string Code
);