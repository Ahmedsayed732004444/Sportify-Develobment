namespace Sportiva.Persistence.EntitiesConfigurations;

public class SubscriptionPaymentConfiguration : IEntityTypeConfiguration<SubscriptionPayment>
{
    public void Configure(EntityTypeBuilder<SubscriptionPayment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Amount)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(x => x.Status)
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.Property(x => x.TransactionId).HasMaxLength(200);

        builder.HasOne(x => x.ClubSubscription)
               .WithMany(cs => cs.Payments)
               .HasForeignKey(x => x.ClubSubscriptionId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
