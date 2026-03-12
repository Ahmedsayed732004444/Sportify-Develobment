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

        // يوزر واحد = Profile واحد
        builder.HasIndex(x => x.UserId).IsUnique();

        builder.HasOne(x => x.User)
               .WithOne()
               .HasForeignKey<UserProfile>(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Posts)
               .WithOne(p => p.UserProfile)
               .HasForeignKey(p => p.UserProfileId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
