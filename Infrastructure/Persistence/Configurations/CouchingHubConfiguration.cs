using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class CouchingHubConfiguration : IEntityTypeConfiguration<CouchingHub>
{
    public void Configure(EntityTypeBuilder<CouchingHub> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasOne(u => u.CouchingHubCategory)
            .WithMany(u => u.CouchingHubs)
            .HasForeignKey(u => u.CouchingHubCategoryId)
           .OnDelete(DeleteBehavior.Cascade);
    }
}
