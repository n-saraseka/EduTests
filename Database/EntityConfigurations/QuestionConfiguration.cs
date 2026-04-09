using EduTests.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduTests.Database.EntityConfigurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder
            .Property(q => q.Description)
            .HasMaxLength(1024);
        builder
            .ToTable(t => 
                t.HasCheckConstraint("order_index_check", "order_index >= 1"));
        builder
            .ComplexProperty(q => q.Data, d => d.ToJson());
        builder
            .ComplexProperty(q => q.CorrectData, d => d.ToJson());
    }
}