namespace Sportiva.Persistence.EntitiesConfigurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Price)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(x => x.Status)
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasIndex(x => new { x.UserId, x.BookingDate });

        // Race condition prevention: ensure only one active booking per TimeSlot
        // This unique constraint is filtered to only count Pending and Confirmed statuses.
        // When a booking is Cancelled or Completed, this constraint no longer applies to it,
        // making the slot available for re-booking automatically (no separate "free up slot" logic needed).
        // 
        // IMPORTANT: This requires a migration with a filtered unique index.
        // In the migration, add:
        //   builder.Sql(
        //       @"CREATE UNIQUE INDEX [IX_Bookings_TimeSlotId_Active] ON [Bookings] ([TimeSlotId])
        //         WHERE [Status] IN ('Pending', 'Confirmed') AND [IsDeleted] = 0");
        // 
        // This prevents two concurrent inserts from both claiming the same slot at the DB level,
        // not just in application code. The second concurrent request will receive a unique constraint
        // violation, which BookingService will catch and translate into TimeSlot.AlreadyBooked (409).
        builder.HasIndex(x => x.TimeSlotId)
               .IsUnique()
               .HasFilter("[Status] IN ('Pending', 'Confirmed')");

        builder.HasOne(x => x.Court)
               .WithMany()
               .HasForeignKey(x => x.CourtId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TimeSlot)
               .WithMany(t => t.Bookings)
               .HasForeignKey(x => x.TimeSlotId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
               .WithMany(u => u.Bookings)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
