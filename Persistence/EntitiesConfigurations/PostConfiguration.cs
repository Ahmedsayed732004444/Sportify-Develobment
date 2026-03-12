namespace Sportiva.Persistence.EntitiesConfigurations;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Content)
               .HasMaxLength(2000)
               .IsRequired();

        builder.Property(x => x.FileUrl).HasMaxLength(500);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne(x => x.UserProfile)
               .WithMany(p => p.Posts)
               .HasForeignKey(x => x.UserProfileId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Likes)
               .WithOne(l => l.Post)
               .HasForeignKey(l => l.PostId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
