using EasyUniAPI.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace EasyUniAPI.Core.Implementations
{
    public class ClaimsProvider : IClaimsProvider
    {
        private readonly HttpContext _httpContext;

        public ClaimsProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor.HttpContext
                ?? throw new ArgumentException("No HttpContext provided!");
        }

        public string? GetLoggedInUserId()
        {
            return _httpContext.User.Identity is { IsAuthenticated: true }
                ? _httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                : null;
        }

        public IReadOnlyList<string> GetLoggedInUserRoles()
        {
            if (_httpContext.User.Identity is null or { IsAuthenticated: false})
            {
                return [];
            }

            return [.. _httpContext.User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)];
        }
    }
}
