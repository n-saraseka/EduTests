using EduTests.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduTests.Database.EntityConfigurations;

public class TestCompletionConfiguration : IEntityTypeConfiguration<TestCompletion>
{
    public void Configure(EntityTypeBuilder<TestCompletion> builder)
    {
        builder
            .HasOne(tc => tc.User)
            .WithMany()
            .HasForeignKey(tc => tc.UserId);
        builder
            .HasOne(tc => tc.AnonymousUser)
            .WithMany()
            .HasForeignKey(tc => tc.AnonymousUserId);
        builder
            .HasOne(tc => tc.Test)
            .WithMany()
            .HasForeignKey(tc => tc.TestId);
        builder
            .ToTable(tc =>
            {
                tc.HasCheckConstraint("completion_percentage_check", "completion_percentage >= 0 AND completion_percentage <= 100");
                tc.HasCheckConstraint("answer_count_check", "correct_answers >= 0");
            });
    }
}