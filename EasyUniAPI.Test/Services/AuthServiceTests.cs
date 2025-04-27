using EasyUniAPI.Common.Configurations;
using EasyUniAPI.Common.Dto;
using EasyUniAPI.Core.Implementations;
using EasyUniAPI.Core.Validators;
using Microsoft.Extensions.Options;

namespace EasyUniAPI.Test.Services
{
    public class AuthServiceTests : BaseServiceTests
    {
        private readonly AuthService _authService;

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
                jwtOptions, userRoleRepository, grantUserRolesValidator, changePasswordValidator);
        }

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
            Assert.Contains("Invalid password.", result.Errors);
        }

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
    }
}
