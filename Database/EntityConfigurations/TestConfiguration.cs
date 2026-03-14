using EduTests.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduTests.Database.EntityConfigurations;

public class TestConfiguration : IEntityTypeConfiguration<Test>
{
    public void Configure(EntityTypeBuilder<Test> builder)
    {
        builder
            .HasMany(t => t.Tags)
            .WithMany();
        builder
            .HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId);
        builder
            .Property(t => t.Name)
            .HasMaxLength(64);
        builder
            .Property(t => t.Description)
            .HasMaxLength(256);
        builder
            .ToTable(t =>
            {
                t.HasCheckConstraint("AttemptLimitCheck", 
                    "AttemptLimit IS NULL OR (AttemptLimit >= 0 AND AttemptLimit <= 10)");
                t.HasCheckConstraint("TimeLimitCheck", 
                    "TimeLimit IS NULL OR (TimeLimit >= INTERVAL '0 hours' AND TimeLimit <=INTERVAL '5 hours')");
            });
        builder
            .HasIndex(t => t.Name);
        builder
            .HasIndex(t => t.Description);
    }
}