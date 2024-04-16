using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class SubscriptionHeadingConfiguration : IEntityTypeConfiguration<SubscriptionHeading>
{
    public void Configure(EntityTypeBuilder<SubscriptionHeading> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasOne(u => u.Subscription)
            .WithMany(u => u.SubscriptionHeadings)
            .HasForeignKey(u => u.SubscriptionId)
           .OnDelete(DeleteBehavior.Cascade);

    }
}