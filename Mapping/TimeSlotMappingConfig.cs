using Mapster;
using Sportiva.Contracts.Shared.Summaries;
using Sportiva.Entities;
using Sportiva.Enums;
using System.Linq;

namespace Sportiva.Mapping;

public class TimeSlotMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<TimeSlot, TimeSlotSummary>()
            .Map(dest => dest.TimeSlotId, src => src.Id)
            .Map(dest => dest.IsBooked, src => src.Bookings.Any(b =>
                !b.IsDeleted &&
                (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending)));
    }
}
