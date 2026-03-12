namespace Sportiva.Persistence.EntitiesConfigurations;

public class FriendlyMatchConfiguration : IEntityTypeConfiguration<FriendlyMatch>
{
    public void Configure(EntityTypeBuilder<FriendlyMatch> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.SportType)
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.Property(x => x.Status)
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.Property(x => x.Note).HasMaxLength(500);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasIndex(x => new { x.Date, x.Status });

        builder.HasOne(x => x.Organizer)
               .WithMany(u => u.OrganizedMatches)
               .HasForeignKey(x => x.OrganizerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Court)
               .WithMany()
               .HasForeignKey(x => x.CourtId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.JoinRequests)
               .WithOne(r => r.FriendlyMatch)
               .HasForeignKey(r => r.FriendlyMatchId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
