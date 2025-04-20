using EasyUniAPI.Common.Dto;

namespace EasyUniAPI.Core.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResultDto<string>> LoginAsync(LoginDto loginDto);
        Task<ServiceResultDto> RegisterAsync(RegisterDto registerDto);
    }
}
