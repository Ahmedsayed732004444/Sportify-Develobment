using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sportiva.Persistence.EntitiesConfigurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {
        builder.HasData(new IdentityUserRole<string>
        {
            UserId = DefaultUsers.Admin.Id,
            RoleId = DefaultRoles.Admin.Id
        }, new IdentityUserRole<string>
        {
            UserId = DefaultUsers.TesterAdmin.Id,
            RoleId = DefaultRoles.Admin.Id
        });
    }
}
