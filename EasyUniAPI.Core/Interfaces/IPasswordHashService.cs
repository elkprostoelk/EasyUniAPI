namespace EasyUniAPI.Core.Interfaces
{
    public interface IPasswordHashService
    {
        (string hash, string salt) HashPassword(string password, string? oldSalt = null);
    }
}
