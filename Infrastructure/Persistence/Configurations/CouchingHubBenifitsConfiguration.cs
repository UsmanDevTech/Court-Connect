using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class CouchingHubBenifitsConfiguration : IEntityTypeConfiguration<CouchingHubBenifit>
{
    public void Configure(EntityTypeBuilder<CouchingHubBenifit> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasOne(u => u.CouchingHub)
            .WithMany(u => u.couchingHubBenifits)
            .HasForeignKey(u => u.CouchingHubId)
           .OnDelete(DeleteBehavior.NoAction);
    }
}
