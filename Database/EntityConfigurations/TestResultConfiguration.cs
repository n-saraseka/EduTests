using EduTests.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduTests.Database.EntityConfigurations;

public class TestResultConfiguration : IEntityTypeConfiguration<TestResult>
{
    public void Configure(EntityTypeBuilder<TestResult> builder)
    {
        builder
            .Property(tr => tr.Result)
            .HasMaxLength(32);
        builder
            .ToTable(t => 
                t.HasCheckConstraint("threshold_check", "percentage_threshold >= 0 AND percentage_threshold <= 100"));
    }
}