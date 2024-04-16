
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class LeagueRewardConfiguration : IEntityTypeConfiguration<LeagueReward>
{
    public void Configure(EntityTypeBuilder<LeagueReward> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasOne(u => u.SubLeague)
            .WithMany(u => u.LeagueRewards)
            .HasForeignKey(u => u.SubLeagueId)
           .OnDelete(DeleteBehavior.Cascade);
    }
}