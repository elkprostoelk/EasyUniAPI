using EasyUniAPI.Common.Dto;
using EasyUniAPI.Core.Interfaces;
using EasyUniAPI.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyUniAPI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ServiceResultDto<string>), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            return result.IsSuccess
                ? Ok(result)
                : Conflict(result);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("register")]
        [ProducesResponseType(typeof(ServiceResultDto), StatusCodes.Status201Created)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);
            return result.IsSuccess
                ? Created()
                : Conflict(result);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPatch("grant-roles")]
        [ProducesResponseType(typeof(ServiceResultDto), StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GrantUserRoles(GrantUserRolesDto grantUserRolesDto)
        {
            var result = await _authService.GrantUserRolesAsync(grantUserRolesDto);
            return result.IsSuccess
                ? NoContent()
                : Conflict(result);
        }

        [Authorize]
        [HttpPatch("change-password")]
        [ProducesResponseType(typeof(ServiceResultDto), StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var currentUserId = User.GetUserId();
            if (currentUserId != changePasswordDto.UserId && !User.IsInRoles("Administrator"))
            {
                return Forbid();
            }

            var result = await _authService.ChangePasswordAsync(changePasswordDto);
            return result.IsSuccess
                ? NoContent()
                : Conflict(result);
        }
    }
}
