using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations;

public sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasOne(u => u.MatchJoinRequest)
            .WithMany(u => u.Notifications)
            .HasForeignKey(u => u.MatchJoinRequestId)
           .OnDelete(DeleteBehavior.NoAction);

    }
}
