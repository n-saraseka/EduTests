using EduTests.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduTests.Database.EntityConfigurations;

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
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
            .HasOne(r => r.Comment)
            .WithMany()
            .HasForeignKey(r => r.CommentId);
        builder
            .Property(r => r.Text)
            .HasMaxLength(256);
        builder
            .ToTable(t => 
                t.HasCheckConstraint("SingleTargetCheckReport", 
                    "(test_id IS NOT NULL AND user_id IS NULL AND comment_id IS NULL) OR " +
                    "(test_id IS NULL AND user_id IS NOT NULL AND comment_id IS NULL) OR " +
                    "(test_id IS NULL AND user_id IS NULL AND comment_id IS NOT NULL)"));
    }
}