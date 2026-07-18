namespace Sportiva.Persistence.EntitiesConfigurations;

public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .HasMaxLength(100)
               .IsRequired();
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.Description).HasMaxLength(500);

        builder.Property(x => x.Price)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(x => x.DurationInDays)
               .IsRequired();

        builder.HasMany(x => x.ClubSubscriptions)
               .WithOne(s => s.Plan)
               .HasForeignKey(s => s.PlanId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new SubscriptionPlan
            {
                Id = "0191b2c3-c4fc-752e-9d95-40b30fa7a9b1",
                Name = "Basic Plan",
                Description = "Perfect for local single-pitch venues. Monitor 1 club and host 1 active tournament.",
                Price = 100m,
                MaxCourts = 1,
                DurationInDays = 365,
                IsActive = true,
                IsDeleted = false,
                CreatedAt = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new SubscriptionPlan
            {
                Id = "0191b2c3-c4fc-752e-9d95-40b30fa7a9b2",
                Name = "Premium Plan",
                Description = "Perfect for growing clubs. Manage up to 2 clubs and host 3 active tournaments simultaneously.",
                Price = 250m,
                MaxCourts = 3,
                DurationInDays = 365,
                IsActive = true,
                IsDeleted = false,
                CreatedAt = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new SubscriptionPlan
            {
                Id = "0191b2c3-c4fc-752e-9d95-40b30fa7a9b3",
                Name = "Elite Plan",
                Description = "For sports centers and large complexes. Manage up to 5 clubs and host 10 active tournaments.",
                Price = 500m,
                MaxCourts = 10,
                DurationInDays = 365,
                IsActive = true,
                IsDeleted = false,
                CreatedAt = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}