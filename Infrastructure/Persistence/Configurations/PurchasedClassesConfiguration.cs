
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class PurchasedClassesConfiguration : IEntityTypeConfiguration<PurchasedClasses>
{
    public void Configure(EntityTypeBuilder<PurchasedClasses> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasOne(u => u.CouchingHub)
            .WithMany(u => u.PurchasedClassess)
            .HasForeignKey(u => u.CouchingHubId)
           .OnDelete(DeleteBehavior.NoAction);
    }
}
