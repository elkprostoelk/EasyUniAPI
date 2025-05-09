using EasyUniAPI.Common.Dto;

namespace EasyUniAPI.Core.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResultDto<string>> LoginAsync(LoginDto loginDto);
        Task<ServiceResultDto> UnlockUserAsync(string userId);
        Task<ServiceResultDto> RegisterAsync(RegisterDto registerDto);
        Task<ServiceResultDto> GrantUserRolesAsync(GrantUserRolesDto grantUserRolesDto);
        Task<ServiceResultDto> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
    }
}
