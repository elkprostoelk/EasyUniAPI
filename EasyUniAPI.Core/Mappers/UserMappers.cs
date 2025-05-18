using EasyUniAPI.Common.Dto;
using EasyUniAPI.DataAccess.Entities;

namespace EasyUniAPI.Core.Mappers
{
    public static class UserMappers
    {
        public static UserProfileDto MapUserToUserProfileDto(this User user)
        {
            if (user is null)
            {
                return new UserProfileDto();
            }

            return new UserProfileDto
            {
                Gender = user.Gender,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
                BirthDate = user.BirthDate,
                PhoneNumber = user.PhoneNumber
            };
        }

        public static User MapRegisterDtoToUser(this RegisterDto registerDto)
        {
            if (registerDto is null)
            {
                return new User();
            }

            return new User
            {
                Gender = registerDto.Gender,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                MiddleName = registerDto.MiddleName,
                PhoneNumber = registerDto.PhoneNumber,
                BirthDate = registerDto.BirthDate
            };
        }
    }
}
