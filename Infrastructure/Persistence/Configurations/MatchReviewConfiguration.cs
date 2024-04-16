using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class MatchReviewConfiguration : IEntityTypeConfiguration<MatchReview>
{
    public void Configure(EntityTypeBuilder<MatchReview> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasOne(u => u.TennisMatch)
            .WithMany(u => u.MatchReviews)
            .HasForeignKey(u => u.TennisMatchId)
           .OnDelete(DeleteBehavior.NoAction);
    }
}
