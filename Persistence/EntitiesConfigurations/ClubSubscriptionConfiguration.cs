namespace Sportiva.Persistence.EntitiesConfigurations;

public class ClubSubscriptionConfiguration : IEntityTypeConfiguration<ClubSubscription>
{
    public void Configure(EntityTypeBuilder<ClubSubscription> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Ignore(x => x.IsActive); // Computed property

        builder.HasIndex(x => new { x.ClubId, x.EndDate });

        builder.HasOne(x => x.Plan)
               .WithMany(p => p.ClubSubscriptions)
               .HasForeignKey(x => x.PlanId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Payments)
               .WithOne(p => p.ClubSubscription)
               .HasForeignKey(p => p.ClubSubscriptionId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
