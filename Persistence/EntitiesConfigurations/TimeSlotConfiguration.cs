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

        // Unique constraint on (CourtId, Day, StartTime) ensures no exact duplicates.
        // Combined with application-level overlap checking, this prevents race conditions
        // when creating overlapping but non-identical time slots concurrently.
        // Assumption: slots are fixed-duration, grid-aligned (e.g., hourly grid).
        // If slot duration becomes variable, upgrade to pessimistic locking (SERIALIZABLE transactions).
        builder.HasIndex(x => new { x.CourtId, x.Day, x.StartTime }).IsUnique();

        builder.HasIndex(x => new { x.CourtId, x.Day });

        builder.HasMany(x => x.Bookings)
               .WithOne(b => b.TimeSlot)
               .HasForeignKey(b => b.TimeSlotId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
