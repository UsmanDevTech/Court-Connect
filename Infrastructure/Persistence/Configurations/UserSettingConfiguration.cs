using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class UserSettingConfiguration : IEntityTypeConfiguration<UserSetting>
{
    public void Configure(EntityTypeBuilder<UserSetting> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasOne(u => u.SubLeague)
            .WithMany(u => u.UserSettings)
            .HasForeignKey(u => u.SubLeagueId)
           .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(u => u.Subscription)
            .WithMany(u => u.UserSettings)
            .HasForeignKey(u => u.SubscriptionId)
           .OnDelete(DeleteBehavior.NoAction);
    }
}