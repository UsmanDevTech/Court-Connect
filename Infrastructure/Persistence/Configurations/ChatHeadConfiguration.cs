using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class ChatHeadConfiguration : IEntityTypeConfiguration<ChatHead>
{
    public void Configure(EntityTypeBuilder<ChatHead> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasOne(u => u.TennisMatch)
            .WithMany(u => u.ChatHeads)
            .HasForeignKey(u => u.TennisMatchId)
           .OnDelete(DeleteBehavior.NoAction);
    }
}