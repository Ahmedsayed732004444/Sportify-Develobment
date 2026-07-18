namespace Sportiva.Errors;

public record TimeSlotErrors
{
    public static readonly Error Error =
        new("TimeSlots.Error", "An error occurred while processing the time slot", StatusCodes.Status500InternalServerError);

    public static readonly Error TimeSlotNotFound =
        new("TimeSlots.NotFound", "The specified time slot was not found", StatusCodes.Status404NotFound);

    public static readonly Error SomeSlotsNotFound =
        new("TimeSlots.SomeNotFound", "One or more of the specified time slots were not found for this court", StatusCodes.Status404NotFound);

    public static readonly Error Unauthorized =
        new("TimeSlots.Unauthorized", "You are not authorized to manage time slots for this court", StatusCodes.Status403Forbidden);

    public static readonly Error CourtNotActive =
        new("TimeSlots.CourtNotActive", "Cannot generate time slots for an inactive court", StatusCodes.Status409Conflict);
}