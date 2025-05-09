using NUlid;

namespace EasyUniAPI.DataAccess.Entities
{
    public class User
    {
        public string Id { get; set; } = Ulid.NewUlid().ToString();

        public required string Email { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public string? MiddleName { get; set; }

        public DateOnly BirthDate { get; set; }

        public required string PhoneNumber { get; set; }

        public string PasswordSalt { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public bool Active { get; set; } = true;

        public int FailedLoginAttempts { get; set; }

        public virtual List<Role> Roles { get; set; } = [];
    }
}
