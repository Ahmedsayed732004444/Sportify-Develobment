namespace Sportiva.Contracts.Bookings;

public record CreateBookingRequest(
    string CourtId,
    string TimeSlotId
);
