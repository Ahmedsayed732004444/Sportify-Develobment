namespace Sportiva.Persistence.EntitiesConfigurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Rating)
               .IsRequired();

        // Rating لازم يكون بين 1 و 5
        builder.ToTable(t => t.HasCheckConstraint("CK_Review_Rating", "[Rating] >= 1 AND [Rating] <= 5"));

        builder.Property(x => x.Comment).HasMaxLength(1000);

        builder.HasQueryFilter(x => !x.IsDeleted);

        // يوزر واحد يعمل review واحد على كل booking
        builder.HasIndex(x => new { x.UserId, x.BookingId }).IsUnique();

        builder.HasOne(x => x.Court)
               .WithMany()
               .HasForeignKey(x => x.CourtId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Booking)
               .WithMany()
               .HasForeignKey(x => x.BookingId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
