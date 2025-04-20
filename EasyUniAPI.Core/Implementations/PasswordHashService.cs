using EasyUniAPI.Core.Interfaces;
using System.Security.Cryptography;

namespace EasyUniAPI.Core.Implementations
{
    public class PasswordHashService : IPasswordHashService
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100_000;

        public (string hash, string salt) HashPassword(string password, string? oldSalt = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(password);
            byte[] saltBytes;
            if (string.IsNullOrWhiteSpace(oldSalt))
            {
                using var rng = RandomNumberGenerator.Create();
                saltBytes = new byte[SaltSize];
                rng.GetBytes(saltBytes);
            }
            else
            {
                saltBytes = Convert.FromBase64String(oldSalt);
            }

            var hashBytes = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA256)
                .GetBytes(HashSize);

            string saltBase64 = !string.IsNullOrWhiteSpace(oldSalt)
                ? oldSalt
                : Convert.ToBase64String(saltBytes);
            string hashBase64 = Convert.ToBase64String(hashBytes);

            return (hashBase64, saltBase64);
        }
    }
}
