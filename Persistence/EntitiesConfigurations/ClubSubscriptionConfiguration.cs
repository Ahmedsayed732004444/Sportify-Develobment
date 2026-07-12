namespace Sportiva.Persistence.EntitiesConfigurations;

public class ClubSubscriptionConfiguration : IEntityTypeConfiguration<ClubSubscription>
{
    public void Configure(EntityTypeBuilder<ClubSubscription> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Price)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(x => x.RefundAmount)
               .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Status)
               .HasConversion<string>()
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(x => x.CreatedAt)
               .HasDefaultValueSql("GETUTCDATE()");

        // Indexes for common queries
        builder.HasIndex(x => new { x.UserId, x.ClubId, x.Status });
        builder.HasIndex(x => new { x.UserId, x.Status });
        builder.HasIndex(x => new { x.ClubId, x.Status });

        builder.HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Club)
               .WithMany(c => c.Subscriptions)
               .HasForeignKey(x => x.ClubId)
               .OnDelete(DeleteBehavior.Restrict);

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
