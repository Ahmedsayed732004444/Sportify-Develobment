namespace Sportiva.Persistence.EntitiesConfigurations;

public class PostLikeConfiguration : IEntityTypeConfiguration<PostLike>
{
    public void Configure(EntityTypeBuilder<PostLike> builder)
    {
        builder.HasKey(x => x.Id);

        // يوزر واحد يعمل like واحد على كل post
        builder.HasIndex(x => new { x.PostId, x.UserId }).IsUnique();

        builder.HasOne(x => x.Post)
               .WithMany(p => p.Likes)
               .HasForeignKey(x => x.PostId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
