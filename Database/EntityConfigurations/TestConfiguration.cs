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
        builder.HasMany(t => t.Questions)
            .WithOne(q => q.Test)
            .HasForeignKey(q => q.TestId);
        builder.HasMany(t => t.Results)
            .WithOne(r => r.Test)
            .HasForeignKey(r => r.TestId);
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
                t.HasCheckConstraint("attempt_limit_check", 
                    "attempt_limit IS NULL OR (attempt_limit >= 0 AND attempt_limit <= 10)");
                t.HasCheckConstraint("time_limit_check", 
                    "time_limit IS NULL OR (time_limit >= '0 hours'::interval AND time_limit <= '5 hours'::interval)");
            });
        builder
            .HasIndex(t => t.Name);
        builder
            .HasIndex(t => t.Description);
    }
}