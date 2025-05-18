using EasyUniAPI.Common.Dto;
using EasyUniAPI.Core.Interfaces;
using EasyUniAPI.Core.Mappers;
using EasyUniAPI.DataAccess;
using EasyUniAPI.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace EasyUniAPI.Core.Implementations
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IRepository<User, string> _userRepository;

        public UserProfileService(IRepository<User, string> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ServiceResultDto<UserProfileDto>> GetUserProfileAsync(string userId)
        {
            var user = await _userRepository.DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null)
            {
                return new ServiceResultDto<UserProfileDto>
                {
                    IsSuccess = false,
                    Errors = ["User not found."]
                };
            }

            return new ServiceResultDto<UserProfileDto>
            {
                IsSuccess = true,
                Result = user.MapUserToUserProfileDto()
            };
        }
    }
}
