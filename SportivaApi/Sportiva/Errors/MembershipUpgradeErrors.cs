namespace Sportiva.Errors;

public record MembershipUpgradeErrors
{
    public static readonly Error Error =
        new("MembershipUpgrades.Error", "An error occurred while processing the membership upgrade request", StatusCodes.Status500InternalServerError);

    public static readonly Error RequestNotFound =
        new("MembershipUpgrades.NotFound", "The specified membership upgrade request was not found", StatusCodes.Status404NotFound);

    // مينفعش تبعت طلب جديد وأنت لسه عندك طلب Pending — "one active request at a time"
    public static readonly Error AlreadyHasPendingRequest =
        new("MembershipUpgrades.AlreadyHasPendingRequest", "You already have a pending membership upgrade request", StatusCodes.Status409Conflict);

    // مينفعش تراجع طلب اتراجع قبل كده (Approved/Rejected)
    public static readonly Error AlreadyReviewed =
        new("MembershipUpgrades.AlreadyReviewed", "This request has already been reviewed", StatusCodes.Status409Conflict);
}