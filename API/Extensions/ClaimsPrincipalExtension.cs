using System.Security.Claims;

namespace Extensions.API;

public static class ClaimsPrincipalExtension
{
    public static string GetUserName(this ClaimsPrincipal user)
    {

        var users=user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return users;

    }
}