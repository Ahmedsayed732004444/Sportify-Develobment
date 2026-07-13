using Mapster;
using Sportiva.Contracts.Bookings;
using Sportiva.Contracts.Shared.Enums;
using Sportiva.Contracts.Shared.Summaries;
using Sportiva.Entities;
using Sportiva.Enums;

namespace Sportiva.Mapping;

public class BookingMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Club to ClubSummary mapping
        config.NewConfig<Club, ClubSummary>()
            .Map(dest => dest.ClubId, src => src.Id);

        // Court to CourtSummary mapping
        config.NewConfig<Court, CourtSummary>()
            .Map(dest => dest.CourtId, src => src.Id)
            .Map(dest => dest.SportType, src => (SportTypeDto)src.SportType);

        // Booking to BookingResponse mapping
        config.NewConfig<Booking, BookingResponse>()
            .Map(dest => dest.BookingId, src => src.Id)
            .Map(dest => dest.BookingNumber, src => "BK-" + src.Id.Substring(0, 8).ToUpper())
            .Map(dest => dest.Status, src => (BookingStatusDto)src.Status)
            .Map(dest => dest.Price, src => src.Price)
            .Map(dest => dest.Court, src => src.Court)
            .Map(dest => dest.TimeSlot, src => src.TimeSlot)
            .Map(dest => dest.BookedBy, src => src.User != null 
                ? new UserSummary(src.User.Id, src.User.FullName, src.User.UserProfile != null ? src.User.UserProfile.ProfilePictureUrl : null)
                : new UserSummary(src.UserId, src.UserId, null))
            .Map(dest => dest.IsMine, src => src.UserId == (string)MapContext.Current.Parameters["currentUserId"])
            .Map(dest => dest.CanCancel, src => src.Status == BookingStatus.Confirmed || src.Status == BookingStatus.Pending)
            .Map(dest => dest.CanReview, src => src.Status == BookingStatus.Completed)
            .Map(dest => dest.ExistingReview, src => (ReviewSummary?)null)
            .Map(dest => dest.CreatedAt, src => src.BookingDate);
    }
}
