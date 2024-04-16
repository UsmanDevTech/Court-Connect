
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class TemporaryMatchScoreConfiguration : IEntityTypeConfiguration<TemporaryMatchScore>
{
    public void Configure(EntityTypeBuilder<TemporaryMatchScore> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasOne(u => u.TennisMatch)
            .WithMany(u => u.TemporaryMatchScores)
            .HasForeignKey(u => u.TennisMatchId)
           .OnDelete(DeleteBehavior.Cascade);
    }
}