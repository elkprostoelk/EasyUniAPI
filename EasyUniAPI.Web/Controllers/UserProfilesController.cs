using EasyUniAPI.Common.Dto;
using EasyUniAPI.Core.Interfaces;
using EasyUniAPI.Web.Extensions;
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

        [HttpPut("{userId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(ServiceResultDto))]
        public async Task<IActionResult> UpdateUserProfile(string userId,
            [FromBody] UpdateUserProfileDto updateUserProfileDto)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }

            if (userId != User.GetUserId() && !User.IsInRole("Administrator"))
            {
                return Forbid();
            }

            var result = await _userProfileService.UpdateUserProfileAsync(userId, updateUserProfileDto);
            return result.IsSuccess
                ? NoContent()
                : Conflict(result);
        }
    }
}
