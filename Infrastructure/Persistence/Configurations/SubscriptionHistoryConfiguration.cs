using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Persistence.Configurations;
public sealed class SubscriptionHistoryConfiguration : IEntityTypeConfiguration<SubscriptionHistory>
{
    public void Configure(EntityTypeBuilder<SubscriptionHistory> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasOne(u => u.Subscription)
            .WithMany(u => u.SubscriptionHistories)
            .HasForeignKey(u => u.SubscriptionId)
           .OnDelete(DeleteBehavior.Cascade);
    }
}