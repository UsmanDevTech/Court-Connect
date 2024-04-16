using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class SubLeagueConfiguration : IEntityTypeConfiguration<SubLeague>
{
    public void Configure(EntityTypeBuilder<SubLeague> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasOne(u => u.League)
            .WithMany(u => u.SubLeagues)
            .HasForeignKey(u => u.LeagueId)
           .OnDelete(DeleteBehavior.Cascade);
    }
}