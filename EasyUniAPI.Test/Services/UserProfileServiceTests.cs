using EasyUniAPI.Common.Dto;
using EasyUniAPI.Core.Implementations;
using EasyUniAPI.Core.Validators;
using Microsoft.EntityFrameworkCore;

namespace EasyUniAPI.Test.Services
{
    public class UserProfileServiceTests : BaseServiceTests
    {
        private readonly UserProfileService _userProfileService;

        public UserProfileServiceTests()
        {
            var updateUserValidator = new UpdateUserProfileValidator(userRepository);
            _userProfileService = new UserProfileService(userRepository, updateUserValidator);
        }

        [Fact]
        public async Task GetUserProfileAsync_ValidUserId_ReturnsUserProfile()
        {
            // Act

            var result = await _userProfileService.GetUserProfileAsync("01JST5PR09DKBYK0FJKSPW61VT");

            // Assert

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Result);
            Assert.Equal("Teacher Teacherovich Never", $"{result.Result.FirstName} {result.Result.MiddleName} {result.Result.LastName}");
        }

        [Fact]
        public async Task GetUserProfileAsync_InvalidUserId_ReturnsError()
        {
            // Act
            var result = await _userProfileService.GetUserProfileAsync("InvalidUserId");

            // Assert

            Assert.False(result.IsSuccess);
            Assert.Null(result.Result);
            Assert.Contains("User not found.", result.Errors);
        }

        [Fact]
        public async Task UpdateUserProfileAsync_ValidData_SuccessfullyUpdatesInfo()
        {
            // Arrange
            var updateUserProfileDto = new UpdateUserProfileDto
            {
                FirstName = "UpdatedFirstName",
                LastName = "UpdatedLastName",
                MiddleName = "UpdatedMiddleName",
                PhoneNumber = "+1234567890",
                Email = "newemail@gmail.com",
                BirthDate = new DateOnly(1997, 1, 2)
            };

            // Act
            var result = await _userProfileService.UpdateUserProfileAsync("01JST5PR09DKBYK0FJKSPW61VT", updateUserProfileDto);

            // Assert

            var user = await userRepository.DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == "01JST5PR09DKBYK0FJKSPW61VT");

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Errors);
            Assert.NotNull(user);
            Assert.Equal("UpdatedFirstName", user.FirstName);
            Assert.Equal("UpdatedLastName", user.LastName);
            Assert.Equal("UpdatedMiddleName", user.MiddleName);
            Assert.Equal("+1234567890", user.PhoneNumber);
            Assert.Equal("newemail@gmail.com", user.Email);
            Assert.Equal(new DateOnly(1997, 1, 2), user.BirthDate);
        }

        [Fact]
        public async Task UpdateUserProfileAsync_UserNotFound_ReturnsUnsuccess()
        {
            // Act

            var result = await _userProfileService.UpdateUserProfileAsync("InvalidUserId", new UpdateUserProfileDto());

            // Assert

            Assert.False(result.IsSuccess);
            Assert.Contains("User not found.", result.Errors);
        }
    }
}
