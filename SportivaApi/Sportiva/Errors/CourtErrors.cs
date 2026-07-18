namespace Sportiva.Errors;

public record CourtErrors
{
    public static readonly Error Error =
        new("Courts.Error", "An error occurred while processing the court", StatusCodes.Status500InternalServerError);

    public static readonly Error CourtNotFound =
        new("Courts.NotFound", "The specified court was not found", StatusCodes.Status404NotFound);

    public static readonly Error Unauthorized =
        new("Courts.Unauthorized", "You are not authorized to manage this court", StatusCodes.Status403Forbidden);

    // مفيش اشتراك فعّال للنادي أصلاً — مينفعش يضيف ملاعب من غير خطة
    public static readonly Error NoActiveSubscription =
        new("Courts.NoActiveSubscription",
            "This club has no active subscription. Subscribe to a plan before adding courts.",
            StatusCodes.Status404NotFound);

    // وصل لحد الملاعب المسموح بيه في خطته — رسالة ديناميكية بترجع الحد بالظبط
    public static Error MaxCourtsReached(int maxCourts) =>
        new("Courts.MaxCourtsReached",
            $"Your subscription plan allows up to {maxCourts} courts. Upgrade your plan to add more.",
            StatusCodes.Status403Forbidden);
}