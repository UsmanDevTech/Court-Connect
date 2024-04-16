using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Persistence.Configurations;

public sealed class SubscriptionPointConfiguration : IEntityTypeConfiguration<SubscriptionPoint>
{
    public void Configure(EntityTypeBuilder<SubscriptionPoint> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasOne(u => u.SubscriptionHeading)
            .WithMany(u => u.SubscriptionPoints)
            .HasForeignKey(u => u.SubscriptionHeadingId)
           .OnDelete(DeleteBehavior.Cascade);
     
    }
}