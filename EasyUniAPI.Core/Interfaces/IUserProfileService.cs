using EasyUniAPI.Common.Dto;

namespace EasyUniAPI.Core.Interfaces
{
    public interface IUserProfileService
    {
        Task<ServiceResultDto<UserProfileDto>> GetUserProfileAsync(string userId);

        Task<ServiceResultDto> UpdateUserProfileAsync(string userId, UpdateUserProfileDto updateUserProfileDto);
    }
}
