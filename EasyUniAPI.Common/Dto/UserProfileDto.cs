namespace EasyUniAPI.Common.Dto
{
    public class UserProfileDto
    {
        public required string Email { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public string? MiddleName { get; set; }

        public DateOnly BirthDate { get; set; }

        public required string PhoneNumber { get; set; }
    }
}
