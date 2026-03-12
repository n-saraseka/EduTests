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
            .HasOne(tc => tc.Test)
            .WithMany()
            .HasForeignKey(tc => tc.TestId);
        builder
            .ToTable(tc =>
            {
                tc.HasCheckConstraint("CompletionPercentageCheck", "CompletionPercentage >= 0 AND CompletionPercentage <= 100");
                tc.HasCheckConstraint("AnswerCountCheck", "CorrectAnswers >= 0");
            });
    }
}