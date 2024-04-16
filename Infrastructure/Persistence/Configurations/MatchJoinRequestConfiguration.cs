
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class MatchJoinRequestConfiguration : IEntityTypeConfiguration<MatchJoinRequest>
{
    public void Configure(EntityTypeBuilder<MatchJoinRequest> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasOne(u => u.TennisMatch)
            .WithMany(u => u.MatchJoinRequests)
            .HasForeignKey(u => u.TennisMatchId)
           .OnDelete(DeleteBehavior.NoAction);
    }
}