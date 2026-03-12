namespace Sportiva.Persistence.EntitiesConfigurations;

public class TimeSlotConfiguration : IEntityTypeConfiguration<TimeSlot>
{
    public void Configure(EntityTypeBuilder<TimeSlot> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Day).IsRequired();
        builder.Property(x => x.StartTime).IsRequired();
        builder.Property(x => x.EndTime).IsRequired();

        builder.HasQueryFilter(x => !x.IsDeleted);

        // Index لتسريع البحث عن slots متاحة في يوم معين
        builder.HasIndex(x => new { x.CourtId, x.Day, x.IsBooked });

        builder.HasMany(x => x.Bookings)
               .WithOne(b => b.TimeSlot)
               .HasForeignKey(b => b.TimeSlotId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
