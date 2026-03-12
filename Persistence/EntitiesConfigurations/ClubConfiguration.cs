namespace Sportiva.Persistence.EntitiesConfigurations;

public class ClubConfiguration : IEntityTypeConfiguration<Club>
{
    public void Configure(EntityTypeBuilder<Club> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(x => x.Governorate).HasMaxLength(100);
        builder.Property(x => x.City).HasMaxLength(100);
        builder.Property(x => x.Address).HasMaxLength(300);
        builder.Property(x => x.PhoneNumber).HasMaxLength(20);
        builder.Property(x => x.Email).HasMaxLength(150);
        builder.Property(x => x.LogoUrl).HasMaxLength(500);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasMany(x => x.Courts)
               .WithOne(c => c.Club)
               .HasForeignKey(c => c.ClubId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Subscriptions)
               .WithOne(s => s.Club)
               .HasForeignKey(s => s.ClubId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
