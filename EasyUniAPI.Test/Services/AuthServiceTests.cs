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
            var registerValidator = new RegisterDtoValidator();
            var grantUserRolesValidator = new GrantUserRolesDtoValidator(userRepository, roleRepository);
            _authService = new AuthService(loginValidator, registerValidator, new PasswordHashService(), userRepository, jwtOptions, userRoleRepository, grantUserRolesValidator);
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
    }
}
