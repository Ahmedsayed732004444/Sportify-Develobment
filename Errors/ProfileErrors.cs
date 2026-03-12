namespace Sportiva.Errors;
public record ProfileErrors
{
    public static readonly Error ProfileNotFound =
       new("User.ProfileNotFound", "User profile not found", StatusCodes.Status404NotFound);
}
