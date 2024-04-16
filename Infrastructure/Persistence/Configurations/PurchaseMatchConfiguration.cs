using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class PurchaseMatchConfiguration : IEntityTypeConfiguration<PurchasedMatch>
{
    public void Configure(EntityTypeBuilder<PurchasedMatch> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasOne(u => u.TennisMatch)
            .WithMany(u => u.purchasedMatches)
            .HasForeignKey(u => u.TennisMatchId)
           .OnDelete(DeleteBehavior.NoAction);
    }
}