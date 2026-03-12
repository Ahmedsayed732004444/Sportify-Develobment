namespace Sportiva.Persistence.EntitiesConfigurations;

public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(x => x.Description).HasMaxLength(500);

        builder.Property(x => x.MonthlyPrice)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.HasMany(x => x.ClubSubscriptions)
               .WithOne(s => s.Plan)
               .HasForeignKey(s => s.PlanId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
