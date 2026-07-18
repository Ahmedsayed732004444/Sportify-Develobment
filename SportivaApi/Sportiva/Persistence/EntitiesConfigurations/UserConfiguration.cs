using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sportiva.Entities;
using Sportiva.Abstractions.Consts;
using System;

namespace Sportiva.Persistence.EntitiesConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasMany(x => x.RefreshTokens)
            .WithOne()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.RefreshTokens).AutoInclude(false);

        builder.Property(x => x.FirstName).HasMaxLength(100);
        builder.Property(x => x.LastName).HasMaxLength(100);

        builder.HasMany(x => x.Following)
            .WithOne(f => f.Follower)
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Followers)
            .WithOne(f => f.Following)
            .HasForeignKey(f => f.FollowingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(new ApplicationUser
        {
            Id = DefaultUsers.Admin.Id,
            FirstName = "Sportify",
            LastName = "Admin",
            UserName = DefaultUsers.Admin.Email,
            NormalizedUserName = DefaultUsers.Admin.Email.ToUpper(),
            Email = DefaultUsers.Admin.Email,
            NormalizedEmail = DefaultUsers.Admin.Email.ToUpper(),
            SecurityStamp = DefaultUsers.Admin.SecurityStamp,
            ConcurrencyStamp = DefaultUsers.Admin.ConcurrencyStamp,
            EmailConfirmed = true,
            PasswordHash = DefaultUsers.Admin.PasswordHash,
            CreatedAt = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc)
        }, new ApplicationUser
        {
            Id = DefaultUsers.TesterAdmin.Id,
            FirstName = "Tester",
            LastName = "Admin",
            UserName = DefaultUsers.TesterAdmin.Email,
            NormalizedUserName = DefaultUsers.TesterAdmin.Email.ToUpper(),
            Email = DefaultUsers.TesterAdmin.Email,
            NormalizedEmail = DefaultUsers.TesterAdmin.Email.ToUpper(),
            SecurityStamp = DefaultUsers.TesterAdmin.SecurityStamp,
            ConcurrencyStamp = DefaultUsers.TesterAdmin.ConcurrencyStamp,
            EmailConfirmed = true,
            PasswordHash = DefaultUsers.TesterAdmin.PasswordHash,
            CreatedAt = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}
