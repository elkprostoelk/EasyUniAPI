using System.Security.Claims;

namespace EasyUniAPI.Web.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            if (user is null)
            {
                return string.Empty;
            }
            var claim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return claim?.Value ?? string.Empty;
        }

        public static bool IsInRoles(this ClaimsPrincipal user, params string[] roles)
        {
            ArgumentNullException.ThrowIfNull(user);

            if (roles is null or { Length: 0 })
            {
                return false;
            }

            return roles.All(user.IsInRole);
        }
    }
}
