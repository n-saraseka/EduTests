using EduTests.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduTests.Database.EntityConfigurations;

public class TestResultConfiguration : IEntityTypeConfiguration<TestResult>
{
    public void Configure(EntityTypeBuilder<TestResult> builder)
    {
        builder
            .HasOne(tr => tr.Test)
            .WithMany()
            .HasForeignKey(tr => tr.TestId);
        builder
            .Property(tr => tr.Result)
            .HasMaxLength(32);
        builder
            .ToTable(t => 
                t.HasCheckConstraint("ThresholdCheck", "PercentageThreshold >= 0 AND PercentageThreshold <= 100"));
    }
}