using EasyUniAPI.Common.Configurations;
using EasyUniAPI.Common.Dto;
using EasyUniAPI.Core.Implementations;
using EasyUniAPI.Core.Interfaces;
using EasyUniAPI.Core.Validators;
using Microsoft.Extensions.Options;
using Moq;

namespace EasyUniAPI.Test.Services
{
    public class AuthServiceTests : BaseServiceTests
    {
        private readonly AuthService _authService;
        private readonly Mock<IClaimsProvider> _claimsProviderMock = new();

        public AuthServiceTests()
        {
            var jwtOptions = Options.Create(new JwtOptions
            {
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                Key = "143ba536-9d4b-45c5-b2ff-3f04e0ca1157"
            });
            var loginValidator = new LoginDtoValidator();
            var registerValidator = new RegisterDtoValidator(userRepository);
            var grantUserRolesValidator = new GrantUserRolesDtoValidator(userRepository, roleRepository);
            var changePasswordValidator = new ChangePasswordDtoValidator(userRepository);

            _authService = new AuthService(loginValidator, registerValidator, new PasswordHashService(), userRepository,
                jwtOptions, userRoleRepository, grantUserRolesValidator, changePasswordValidator, _claimsProviderMock.Object);
        }

        #region LoginAsync tests

        [Fact]
        public async Task LoginAsync_SuccessfulLogsIn()
        {
            // Arrange

            var loginDto = new LoginDto
            {
                Login = "admin@admin.com",
                Password = "strongPa$$word123"
            };

            // Act

            var result = await _authService.LoginAsync(loginDto);

            // Assert

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Result);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task LoginAsync_NoUserFound()
        {
            // Arrange

            var loginDto = new LoginDto
            {
                Login = "someuser@somemail.com",
                Password = "strongPa$$word123"
            };

            // Act

            var result = await _authService.LoginAsync(loginDto);

            // Assert

            Assert.False(result.IsSuccess);
            Assert.Null(result.Result);
            Assert.Contains("User was not found.", result.Errors);
        }

        [Fact]
        public async Task LoginAsync_InvalidPassword()
        {
            // Arrange

            var loginDto = new LoginDto
            {
                Login = "admin@admin.com",
                Password = "strongPa$$word"
            };

            // Act

            var result = await _authService.LoginAsync(loginDto);

            // Assert

            Assert.False(result.IsSuccess);
            Assert.Null(result.Result);
            Assert.Contains("Invalid password. You have 4 login attempt(s) left.", result.Errors);
        }

        #endregion

        #region RegisterAsync tests

        [Fact]
        public async Task RegisterAsync_SuccessfulRegistration()
        {
            // Arrange

            var registerDto = new RegisterDto
            {
                Email = "newuser@gmail.com",
                FirstName = "NewUser",
                MiddleName = "Userovich",
                LastName = "Never",
                Password = "strongPa$$word345",
                PhoneNumber = "+1876543210",
                BirthDate = new DateOnly(1975, 3, 6),
                RoleId = 2
            };

            // Act

            var result = await _authService.RegisterAsync(registerDto);

            // Assert

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Errors);
        }

        #endregion

        #region GrantUserRolesAsync tests

        [Fact]
        public async Task GrantUserRolesAsync_GrantRolesSuccessfully()
        {
            // Arrange

            var grantUserRolesDto = new GrantUserRolesDto
            {
                UserId = "01JST5PR09DKBYK0FJKSPW61VT",
                RoleIds = [1, 3]
            };

            // Act

            var result = await _authService.GrantUserRolesAsync(grantUserRolesDto);

            // Assert

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task GrantUserRolesAsync_RoleAlreadyExists_ReturnsUnsuccess()
        {
            // Arrange

            var grantUserRolesDto = new GrantUserRolesDto
            {
                UserId = "01JST5PR09DKBYK0FJKSPW61VT",
                RoleIds = [2, 3]
            };

            // Act

            var result = await _authService.GrantUserRolesAsync(grantUserRolesDto);

            // Assert

            Assert.False(result.IsSuccess);
            Assert.Contains("The user already has some of the roles.", result.Errors);
        }

        #endregion

        #region ChangePasswordAsync tests

        [Fact]
        public async Task ChangePasswordAsync_SuccessfullyChangePassword()
        {
            // Arrange

            var changePasswordDto = new ChangePasswordDto
            {
                UserId = "01JST5PR09DKBYK0FJKSPW61VT",
                OldPassword = "strongPa$$word345",
                NewPassword = "strongPa$$word456"
            };

            // Act

            var result = await _authService.ChangePasswordAsync(changePasswordDto);

            // Assert

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task ChangePasswordAsync_OldPasswordInvalid_ReturnsUnsuccess()
        {
            // Arrange

            var changePasswordDto = new ChangePasswordDto
            {
                UserId = "01JST5PR09DKBYK0FJKSPW61VT",
                OldPassword = "strongPa$$word34",
                NewPassword = "strongPa$$word456"
            };

            // Act

            var result = await _authService.ChangePasswordAsync(changePasswordDto);

            // Assert

            Assert.False(result.IsSuccess);
            Assert.Contains("Invalid old password.", result.Errors);
        }

        #endregion

        #region UnlockUserAsync tests

        [Fact]
        public async Task UnlockUserAsync_SuccessfullyUnlockUser()
        {
            // Arrange

            _claimsProviderMock.Setup(s => s.GetLoggedInUserRoles())
                .Returns(["Administrator"]);

            // Act

            var result = await _authService.UnlockUserAsync("01JTV1XEAH8ZT963X8XE3ACJ69");

            // Assert

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task UnlockUserAsync_UserAlreadyActive_ReturnsUnsuccess()
        {
            // Arrange

            _claimsProviderMock.Setup(s => s.GetLoggedInUserRoles())
                .Returns(["Administrator"]);

            // Act

            var result = await _authService.UnlockUserAsync("01JST5PR09DKBYK0FJKSPW61VT");

            // Assert

            Assert.False(result.IsSuccess);
            Assert.Contains("User is already active.", result.Errors);
        }

        #endregion
    }
}
