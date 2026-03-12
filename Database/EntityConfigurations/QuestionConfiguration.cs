using EduTests.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduTests.Database.EntityConfigurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder
            .HasOne(q => q.Test)
            .WithMany()
            .HasForeignKey(q => q.TestId);
        builder
            .Property(q => q.Description)
            .HasMaxLength(1024);
        builder
            .ToTable(t => 
                t.HasCheckConstraint("OrderIndexCheck", "OrderIndex >= 1"));
    }
}