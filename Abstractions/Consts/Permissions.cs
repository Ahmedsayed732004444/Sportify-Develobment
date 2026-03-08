namespace Sportiva.Abstractions.Consts;
public static class Permissions
{
    public static string Type { get; } = "permissions";

    public static IList<string?> GetAllPermissions() =>
        typeof(Permissions).GetFields().Select(x => x.GetValue(x) as string).ToList();
}