namespace Sportiva.Errors;

public record CommentErrors
{
    public static readonly Error Error =
        new("Comments.Error", "An error occurred while processing the comment", StatusCodes.Status500InternalServerError);

    public static readonly Error CommentNotFound =
        new("Comments.NotFound", "The specified comment was not found", StatusCodes.Status404NotFound);

    public static readonly Error Unauthorized =
        new("Comments.Unauthorized", "You are not authorized to perform this action", StatusCodes.Status403Forbidden);

    public static readonly Error AlreadyLiked =
        new("Comments.AlreadyLiked", "You have already liked this comment", StatusCodes.Status400BadRequest);

    public static readonly Error LikeNotFound =
        new("Comments.LikeNotFound", "The specified like was not found", StatusCodes.Status404NotFound);
}

public record ReplyErrors
{
    public static readonly Error Error =
        new("Replies.Error", "An error occurred while processing the reply", StatusCodes.Status500InternalServerError);

    public static readonly Error ReplyNotFound =
        new("Replies.NotFound", "The specified reply was not found", StatusCodes.Status404NotFound);

    public static readonly Error Unauthorized =
        new("Replies.Unauthorized", "You are not authorized to perform this action", StatusCodes.Status403Forbidden);

    public static readonly Error AlreadyLiked =
        new("Replies.AlreadyLiked", "You have already liked this reply", StatusCodes.Status400BadRequest);

    public static readonly Error LikeNotFound =
        new("Replies.LikeNotFound", "The specified like was not found", StatusCodes.Status404NotFound);
}