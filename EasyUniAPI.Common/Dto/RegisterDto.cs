namespace EasyUniAPI.Common.Dto
{
    public class RegisterDto
    {
        public required string Email { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public string? MiddleName { get; set; }

        public required string PhoneNumber { get; set; }

        public DateOnly BirthDate { get; set; }

        public required string Password { get; set; }

        public int RoleId { get; set; }
    }
}
