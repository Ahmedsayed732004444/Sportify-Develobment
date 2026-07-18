namespace Sportiva.Errors;

public record ReviewErrors
{
    public static readonly Error Error =
        new("Reviews.Error", "An error occurred while processing the review", StatusCodes.Status500InternalServerError);

    public static readonly Error ReviewNotFound =
        new("Reviews.NotFound", "The specified review was not found", StatusCodes.Status404NotFound);

    public static readonly Error Unauthorized =
        new("Reviews.Unauthorized", "You are not authorized to perform this action on this review", StatusCodes.Status403Forbidden);

    // الحجز مش موجود أصلاً (أو محذوف) — بنستخدمها بدل ما نستعير BookingErrors.BookingNotFound
    // عشان الـ error code يفضل تحت namespace الـ Reviews ومتسق مع باقي الأخطاء هنا
    public static readonly Error BookingNotFound =
        new("Reviews.BookingNotFound", "The specified booking was not found", StatusCodes.Status404NotFound);

    // مينفعش تعمل review غير لو الحجز بتاعك (مش حجز حد تاني)
    public static readonly Error BookingNotYours =
        new("Reviews.BookingNotYours", "You can only review your own bookings", StatusCodes.Status403Forbidden);

    // مينفعش تعمل review غير لو الحجز خلص (Completed)
    public static readonly Error BookingNotCompleted =
        new("Reviews.BookingNotCompleted", "You can only review a booking after it has been completed", StatusCodes.Status409Conflict);

    // فيه review موجود بالفعل على نفس الـ booking ده
    public static readonly Error AlreadyReviewed =
        new("Reviews.AlreadyReviewed", "You have already reviewed this booking", StatusCodes.Status409Conflict);

    // الـ Rating لازم يكون بين 1 و 5
    public static readonly Error InvalidRating =
        new("Reviews.InvalidRating", "Rating must be between 1 and 5", StatusCodes.Status400BadRequest);
}