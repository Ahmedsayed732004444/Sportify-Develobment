using Sportiva.Contracts.Shared.Enums;
using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Bookings;

public record BookingResponse(
    string           BookingId,
    string           BookingNumber,
    BookingStatusDto Status,
    decimal          Price,

    CourtSummary    Court,
    TimeSlotSummary TimeSlot,

    UserSummary BookedBy,

    bool IsMine,
    bool CanCancel,
    bool CanReview,

    ReviewSummary? ExistingReview,

    DateTime CreatedAt
);
