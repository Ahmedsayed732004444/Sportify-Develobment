using Sportiva.Contracts.Shared.Enums;

namespace Sportiva.Contracts.Bookings;

public record ReviewBookingRequest(
    BookingStatusDto NewStatus // Confirmed أو Rejected بس، الService يتأكد من ده
);