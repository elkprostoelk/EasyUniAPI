using EasyUniAPI.Common.Dto;
using EasyUniAPI.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyUniAPI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfilesController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;

        public UserProfilesController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        [HttpGet("{userId}")]
        [Authorize]
        [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK, "application/json")]
        [ProducesErrorResponseType(typeof(ServiceResultDto<UserProfileDto>))]
        public async Task<IActionResult> GetUserProfile(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }

            var result = await _userProfileService.GetUserProfileAsync(userId);
            return result.IsSuccess
                ? Ok(result.Result)
                : Conflict(result);
        }
    }
}
