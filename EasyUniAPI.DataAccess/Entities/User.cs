using EasyUniAPI.Common.Enums;
using NUlid;

namespace EasyUniAPI.DataAccess.Entities
{
    public class User
    {
        public string Id { get; set; } = Ulid.NewUlid().ToString();

        public Gender Gender { get; set; }

        public string Email { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string? MiddleName { get; set; }

        public DateOnly BirthDate { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

        public string PasswordSalt { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public bool Active { get; set; } = true;

        public int FailedLoginAttempts { get; set; }

        public virtual List<Role> Roles { get; set; } = [];
    }
}
