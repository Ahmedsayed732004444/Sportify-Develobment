namespace Sportiva.Persistence.EntitiesConfigurations;

public class CourtConfiguration : IEntityTypeConfiguration<Court>
{
    public void Configure(EntityTypeBuilder<Court> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.ImageUrl).HasMaxLength(500);

        builder.Property(x => x.PricePerHour)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(x => x.SportType)
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasMany(x => x.TimeSlots)
               .WithOne(t => t.Court)
               .HasForeignKey(t => t.CourtId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
