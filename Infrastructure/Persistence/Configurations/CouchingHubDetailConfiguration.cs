
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class CouchingHubDetailConfiguration : IEntityTypeConfiguration<CouchingHubDetail>
{
    public void Configure(EntityTypeBuilder<CouchingHubDetail> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasOne(u => u.CouchingHub)
            .WithMany(u => u.CouchingHubDetails)
            .HasForeignKey(u => u.CouchingHubId)
           .OnDelete(DeleteBehavior.NoAction);
    }
}
