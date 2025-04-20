using EasyUniAPI.Common.Configurations;
using EasyUniAPI.Common.Dto;
using EasyUniAPI.Core.Interfaces;
using EasyUniAPI.DataAccess;
using EasyUniAPI.DataAccess.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EasyUniAPI.Core.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IValidator<LoginDto> _loginDtoValidator;
        private readonly IPasswordHashService _passwordHashService;
        private readonly IRepository<User, string> _userRepository;
        private readonly JwtOptions _jwtOptions;

        public AuthService(
            IValidator<LoginDto> loginDtoValidator,
            IPasswordHashService passwordHashService,
            IRepository<User, string> userRepository,
            IOptions<JwtOptions> jwtOptions)
        {
            _loginDtoValidator = loginDtoValidator;
            _passwordHashService = passwordHashService;
            _userRepository = userRepository;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<ServiceResultDto<string>> LoginAsync(LoginDto loginDto)
        {
            await _loginDtoValidator.ValidateAndThrowAsync(loginDto);

            var user = await _userRepository.DbSet
                .AsNoTracking()
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == loginDto.Login);
            if (user is null)
            {
                return new ServiceResultDto<string>
                {
                    IsSuccess = false,
                    Errors = ["User was not found."]
                };
            }

            var (hash, _) = _passwordHashService.HashPassword(loginDto.Password, user.PasswordSalt);
            var isPasswordValid = user.PasswordHash == hash;

            return new ServiceResultDto<string>
            {
                IsSuccess = isPasswordValid,
                Errors = isPasswordValid ? [] : ["Invalid password."],
                Result = isPasswordValid ? GenerateToken(user) : null
            };
        }

        private string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: [
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.Email),
                    ..user.Roles.Select(r => new Claim(ClaimTypes.Role, r.Id.ToString(), ClaimValueTypes.Integer32))
                ],
                expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiresInMinutes),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
