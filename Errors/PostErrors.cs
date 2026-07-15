namespace Sportiva.Errors;
public class PostErrors
{
    public static readonly Error Error =
        new("Posts.Error", "An error occurred while processing the post", StatusCodes.Status500InternalServerError);
    public static readonly Error PostNotFound =
        new("Posts.NotFound", "The specified post was not found", StatusCodes.Status404NotFound);
    public static readonly Error Unauthorized =
        new("Posts.Unauthorized", "You are not authorized to perform this action", StatusCodes.Status403Forbidden);
    public static readonly Error LikeNotFound =
        new("Posts.LikeNotFound", "The specified like was not found", StatusCodes.Status404NotFound);
    public static readonly Error AlreadyLiked =
        new("Posts.AlreadyLiked", "You have already liked this post", StatusCodes.Status400BadRequest);
}
