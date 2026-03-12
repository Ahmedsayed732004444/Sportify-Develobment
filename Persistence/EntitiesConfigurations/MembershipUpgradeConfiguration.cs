namespace Sportiva.Persistence.EntitiesConfigurations;

public class MembershipUpgradeConfiguration : IEntityTypeConfiguration<MembershipUpgrade>
{
    public void Configure(EntityTypeBuilder<MembershipUpgrade> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.Property(x => x.Note).HasMaxLength(500);

        builder.HasOne(x => x.User)
               .WithMany(u => u.MembershipUpgradeRequests)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
