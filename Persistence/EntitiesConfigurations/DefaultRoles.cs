namespace Sportiva.Persistence.EntitiesConfigurations; // ✅ تم تصحيح الـ namespace من Sportiva.Abstractions.Consts

public static class DefaultRoles
{
    public partial class Admin
    {
        public const string Name = nameof(Admin);
        public const string Id = "0191a4b6-c4fc-752e-9d95-40b5e4e68054";
        public const string ConcurrencyStamp = "0191a4b6-c4fc-752e-9d95-40b631d1866d";
    }

    // ✅ تم حذف Role الـ Company لأنه مش متضاف في RoleConfiguration ولا في الـ Database Seed
    // لو محتاجه، أضفه في RoleConfiguration بـ HasData

    public partial class Member
    {
        public const string Name = nameof(Member);
        public const string Id = "0191a4b6-c4fc-752e-9d95-40b7a5cb88f0";
        public const string ConcurrencyStamp = "0191a4b6-c4fc-752e-9d95-40b85cf3fd22";
    }
}
