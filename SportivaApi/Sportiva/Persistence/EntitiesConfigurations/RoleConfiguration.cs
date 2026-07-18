using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sportiva.Entities;

namespace Sportiva.Persistence.EntitiesConfigurations;

public class RoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        //Default Data using fully qualified consts to prevent name collision with local configurations helper
        builder.HasData([
            new ApplicationRole
            {
                Id = Sportiva.Abstractions.Consts.DefaultRoles.Admin.Id,
                Name = Sportiva.Abstractions.Consts.DefaultRoles.Admin.Name,
                NormalizedName = Sportiva.Abstractions.Consts.DefaultRoles.Admin.Name.ToUpper(),
                ConcurrencyStamp = Sportiva.Abstractions.Consts.DefaultRoles.Admin.ConcurrencyStamp
            },
            new ApplicationRole
            {
                Id = Sportiva.Abstractions.Consts.DefaultRoles.Member.Id,
                Name = Sportiva.Abstractions.Consts.DefaultRoles.Member.Name,
                NormalizedName = Sportiva.Abstractions.Consts.DefaultRoles.Member.Name.ToUpper(),
                ConcurrencyStamp = Sportiva.Abstractions.Consts.DefaultRoles.Member.ConcurrencyStamp,
                IsDefault = true
            },
            new ApplicationRole
            {
                Id = Sportiva.Abstractions.Consts.DefaultRoles.Owner.Id,
                Name = Sportiva.Abstractions.Consts.DefaultRoles.Owner.Name,
                NormalizedName = Sportiva.Abstractions.Consts.DefaultRoles.Owner.Name.ToUpper(),
                ConcurrencyStamp = Sportiva.Abstractions.Consts.DefaultRoles.Owner.ConcurrencyStamp,
                IsDefault = false
            }
        ]);
    }
}
