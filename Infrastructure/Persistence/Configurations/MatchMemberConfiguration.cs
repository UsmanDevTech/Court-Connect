
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class MatchMemberConfiguration : IEntityTypeConfiguration<MatchMember>
{
    public void Configure(EntityTypeBuilder<MatchMember> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasOne(u => u.TennisMatch)
            .WithMany(u => u.MatchMembers)
            .HasForeignKey(u => u.TennisMatchId)
           .OnDelete(DeleteBehavior.Cascade);
    }
}
