using Microsoft.AspNetCore.Http;
using Sportiva.Abstractions;

namespace Sportiva.Errors;

public static class ClubErrors
{
    public static readonly Error ClubNotFound =
        new("Clubs.NotFound", "The specified club was not found or is deleted", StatusCodes.Status404NotFound);

    public static readonly Error Unauthorized =
        new("Clubs.Unauthorized", "You are not authorized to perform this action on this club", StatusCodes.Status403Forbidden);
}
