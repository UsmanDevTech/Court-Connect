

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class ChatMemberConfiguration : IEntityTypeConfiguration<ChatMember>
{
    public void Configure(EntityTypeBuilder<ChatMember> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasOne(u => u.ChatHead)
            .WithMany(u => u.ChatMembers)
            .HasForeignKey(u => u.ChatHeadId)
           .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(u => u.LastMsgSeen)
            .WithMany(u => u.LastMsgSeens)
            .HasForeignKey(u => u.LastMsgSeenId)
           .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(u => u.ChatHeadDeleteLastMsg)
            .WithMany(u => u.ChatHeadDeleteLastMsgs)
            .HasForeignKey(u => u.ChatHeadDeleteLastMsgId)
           .OnDelete(DeleteBehavior.NoAction);
    }
}