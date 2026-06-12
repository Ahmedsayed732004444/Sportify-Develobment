namespace Sportiva.Persistence.EntitiesConfigurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Bio).HasMaxLength(500);
        builder.Property(x => x.City).HasMaxLength(100);
        builder.Property(x => x.Country).HasMaxLength(100);
        builder.Property(x => x.ProfilePictureUrl).HasMaxLength(500);
        builder.Property(x => x.CoverImageUrl).HasMaxLength(500);
        builder.Property(x => x.PreferredCity).HasMaxLength(100);

        builder.Property(x => x.PreferredSport)
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasIndex(x => x.UserId).IsUnique();

        builder.HasOne(x => x.User)
               .WithOne(x => x.UserProfile)
               .HasForeignKey<UserProfile>(x => x.UserId);

    }
}