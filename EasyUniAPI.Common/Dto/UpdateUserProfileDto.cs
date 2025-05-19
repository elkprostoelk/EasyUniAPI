
using EasyUniAPI.Common.Enums;

namespace EasyUniAPI.Common.Dto
{
    public class UpdateUserProfileDto
    {
        public string Email { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string? MiddleName { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

        public DateOnly BirthDate { get; set; }

        public Gender Gender { get; set; }
    }
}
