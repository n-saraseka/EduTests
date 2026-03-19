using EduTests.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduTests.Database.EntityConfigurations;

public class UserAnswerConfiguration : IEntityTypeConfiguration<UserAnswer>
{
    public void Configure(EntityTypeBuilder<UserAnswer> builder)
    {
        builder
            .HasOne(ua => ua.Completion)
            .WithMany()
            .HasForeignKey(ua => ua.TestCompletionId);
        builder
            .HasOne(ua => ua.Question)
            .WithMany()
            .HasForeignKey(ua => ua.QuestionId);
        builder
            .HasIndex(ua => new {ua.TestCompletionId, ua.QuestionId})
            .IsUnique();
        builder
            .ComplexProperty(ua => ua.Answers, a => a.ToJson());
    }
}