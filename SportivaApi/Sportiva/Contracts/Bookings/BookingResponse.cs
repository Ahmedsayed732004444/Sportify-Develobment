using Sportiva.Contracts.Shared.Enums;
using Sportiva.Contracts.Shared.Summaries;

namespace Sportiva.Contracts.Bookings;

public record BookingResponse(
    string BookingId,
    string BookingNumber,
    BookingStatusDto Status,
    decimal Price,

    CourtSummary Court,
    TimeSlotSummary TimeSlot,

    UserSummary BookedBy,

    bool IsMine,
    bool CanCancel,
    bool CanRespondToRequest,   // ← جديد: true لو انت الـ Owner والحجز لسه Pending
    bool CanReview,             // زي ما هي، خاصة بمراجعة الملعب مش الطلب

    ReviewSummary? ExistingReview,

    DateTime CreatedAt
);