namespace Sportiva.Persistence.EntitiesConfigurations;

public class MatchJoinRequestConfiguration : IEntityTypeConfiguration<MatchJoinRequest>
{
    public void Configure(EntityTypeBuilder<MatchJoinRequest> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
               .HasConversion<string>()
               .HasMaxLength(50);

        // لاعب واحد يقدر يطلب انضمام مرة واحدة لكل ماتش
        builder.HasIndex(x => new { x.FriendlyMatchId, x.PlayerId }).IsUnique();

        builder.HasOne(x => x.FriendlyMatch)
               .WithMany(m => m.JoinRequests)
               .HasForeignKey(x => x.FriendlyMatchId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Player)
               .WithMany(u => u.MatchJoinRequests)
               .HasForeignKey(x => x.PlayerId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
