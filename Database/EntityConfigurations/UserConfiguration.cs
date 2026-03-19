using EduTests.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduTests.Database.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .Property(u => u.Login)
            .HasMaxLength(32);
        builder
            .HasIndex(u => u.Login)
            .IsUnique();
        builder
            .Property(u => u.Username)
            .HasMaxLength(32);
        builder
            .Property(u => u.Description)
            .HasMaxLength(256);
    }
}