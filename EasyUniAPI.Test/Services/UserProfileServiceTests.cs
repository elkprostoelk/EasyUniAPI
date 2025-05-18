using EasyUniAPI.Core.Implementations;

namespace EasyUniAPI.Test.Services
{
    public class UserProfileServiceTests : BaseServiceTests
    {
        private readonly UserProfileService _userProfileService;

        public UserProfileServiceTests()
        {
            _userProfileService = new UserProfileService(userRepository);
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
    }
}
