namespace Sportiva.Errors;

public record ProfileErrors
{
    public static readonly Error Error =
        new("Profile.Error", "An error occurred while processing the profile", StatusCodes.Status500InternalServerError);

    public static readonly Error ProfileNotFound =
        new("Profile.NotFound", "User profile not found", StatusCodes.Status404NotFound);

    public static readonly Error CannotFollowSelf =
        new("Profile.CannotFollowSelf", "You cannot follow yourself", StatusCodes.Status400BadRequest);

    public static readonly Error AlreadyFollowing =
        new("Profile.AlreadyFollowing", "You are already following this user", StatusCodes.Status409Conflict);
}
