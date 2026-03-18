using EduTests.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduTests.Database.EntityConfigurations;

public class UserRatingConfiguration : IEntityTypeConfiguration<UserRating>
{
    public void Configure(EntityTypeBuilder<UserRating> builder)
    {
        builder
            .HasOne(r => r.Test)
            .WithMany()
            .HasForeignKey(r => r.TestId);
        builder
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId);
        builder
            .HasIndex(r => new { r.TestId, r.IsPositive });
    }
}