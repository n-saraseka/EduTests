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
            .HasForeignKey(t => t.TestId);
        builder
            .HasOne(c => c.UserProfile)
            .WithMany()
            .HasForeignKey(t => t.UserProfileId);
        builder
            .Property(c => c.Content)
            .HasMaxLength(256);
        builder
            .ToTable(t => 
                t.HasCheckConstraint("SingleTargetCheckComment", 
                    "(TestId IS NULL AND UserProfileId IS NOT NULL) OR (TestId IS NOT NULL AND UserProfileId IS NULL)"));
    }
}