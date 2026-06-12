namespace Sportiva.Extensions;

public static class UserExtensions
{
    public static string? GetUserId(this ClaimsPrincipal user) =>
       user.FindFirstValue(ClaimTypes.NameIdentifier);
    public static string? GetFirstName(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.GivenName);

    public static string? GetLastName(this ClaimsPrincipal user) =>
        user.FindFirstValue(JwtRegisteredClaimNames.FamilyName);

    public static string? GetFullName(this ClaimsPrincipal user) =>
        user.FindFirstValue(JwtRegisteredClaimNames.Nickname);

    public static string? GetEmail(this ClaimsPrincipal user) =>
        user.FindFirstValue(JwtRegisteredClaimNames.Email);
}