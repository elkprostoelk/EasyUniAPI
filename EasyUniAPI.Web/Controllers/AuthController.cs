using EasyUniAPI.Common.Dto;
using EasyUniAPI.Core.Interfaces;
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
    }
}
