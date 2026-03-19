using EduTests.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduTests.Database.EntityConfigurations;

public class BannedUserConfiguration : IEntityTypeConfiguration<BannedUser>
{
    public void Configure(EntityTypeBuilder<BannedUser> builder)
    {
        builder
            .HasOne(b => b.UserBanned)
            .WithMany()
            .HasForeignKey(b => b.UserBannedId);
        builder
            .HasOne(b => b.BannedBy)
            .WithMany()
            .HasForeignKey(b => b.BannedById);
        builder
            .Property(b => b.BanReason)
            .HasMaxLength(512);
    }
}