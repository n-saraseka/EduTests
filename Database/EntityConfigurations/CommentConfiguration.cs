using EduTests.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduTests.Database.EntityConfigurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder
            .HasOne(c => c.Commenter)
            .WithMany()
            .HasForeignKey(t => t.CommenterId);
        builder
            .HasOne(c => c.Test)
            .WithMany()
            .HasForeignKey(t => t.TestId)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasOne(c => c.UserProfile)
            .WithMany()
            .HasForeignKey(t => t.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .Property(c => c.Content)
            .HasMaxLength(256);
        builder
            .ToTable(t => 
                t.HasCheckConstraint("SingleTargetCheckComment", 
                    "(test_id IS NULL AND user_profile_id IS NOT NULL) OR (test_id IS NOT NULL AND user_profile_id IS NULL)"));
    }
}