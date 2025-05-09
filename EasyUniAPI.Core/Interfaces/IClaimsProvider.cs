namespace EasyUniAPI.Core.Interfaces
{
    public interface IClaimsProvider
    {
        string? GetLoggedInUserId();
        IReadOnlyList<string> GetLoggedInUserRoles();
    }
}
