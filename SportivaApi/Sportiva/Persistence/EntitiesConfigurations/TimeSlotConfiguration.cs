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

        builder.HasIndex(x => new { x.CourtId, x.Day });

        // ✅ يمنع تكرار نفس الـ Slot لنفس الملعب في نفس اليوم والساعة على مستوى
        //    الداتابيز نفسها، مش بس على مستوى الكود (existingSet check) — ده بيحمي
        //    من الـ race condition لو الـ Hangfire job اشتغل مرتين في نفس الوقت.
        builder.HasIndex(x => new { x.CourtId, x.Day, x.StartTime }).IsUnique();

        builder.HasMany(x => x.Bookings)
               .WithOne(b => b.TimeSlot)
               .HasForeignKey(b => b.TimeSlotId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}