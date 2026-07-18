namespace Sportiva.Errors;

public record BookingErrors
{
    public static readonly Error Error =
        new("Bookings.Error", "An error occurred while processing the booking", StatusCodes.Status500InternalServerError);

    public static readonly Error BookingNotFound =
        new("Bookings.NotFound", "The specified booking was not found", StatusCodes.Status404NotFound);

    public static readonly Error TimeSlotNotAvailable =
        new("Bookings.TimeSlotNotAvailable", "This time slot is already booked or pending", StatusCodes.Status409Conflict);

    public static readonly Error Unauthorized =
        new("Bookings.Unauthorized", "You are not authorized to perform this action on this booking", StatusCodes.Status403Forbidden);

    public static readonly Error InvalidStatusTransition =
        new("Bookings.InvalidStatusTransition", "This booking cannot be moved to the requested status", StatusCodes.Status409Conflict);

    public static readonly Error CourtNotActive =
        new("Bookings.CourtNotActive", "Cannot book an inactive court", StatusCodes.Status409Conflict);
}