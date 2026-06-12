namespace Sportiva.Errors;

public record MembershipUpgradeErrors
{
    public static readonly Error PendingUpgradeRequestExists =
        new("MembershipUpgrade.PendingUpgradeRequestExists", "A pending upgrade request already exists for this user.", StatusCodes.Status400BadRequest);
    public static readonly Error Error = new(
        "MembershipUpgrade.Error",
        "An error occurred while processing the membership upgrade request.",
        StatusCodes.Status500InternalServerError);
    public static readonly Error NotFound = new(
        "MembershipUpgrade.NotFound",
        "The requested membership upgrade request was not found.",
        StatusCodes.Status404NotFound);
    public static readonly Error InvalidStatus = new(
        "MembershipUpgrade.InvalidStatus",
        "The membership upgrade request is not in a valid status for this operation.",
        StatusCodes.Status400BadRequest);
}
