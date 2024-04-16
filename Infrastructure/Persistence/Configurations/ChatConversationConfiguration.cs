using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class ChatConversationConfiguration : IEntityTypeConfiguration<ChatConversation>
{
    public void Configure(EntityTypeBuilder<ChatConversation> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasOne(u => u.ChatHead)
            .WithMany(u => u.ChatConversations)
            .HasForeignKey(u => u.ChatHeadId)
           .OnDelete(DeleteBehavior.Cascade);
    }
}