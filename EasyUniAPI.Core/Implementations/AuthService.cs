using EasyUniAPI.Common.Configurations;
using EasyUniAPI.Common.Dto;
using EasyUniAPI.Core.Interfaces;
using EasyUniAPI.DataAccess;
using EasyUniAPI.DataAccess.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EasyUniAPI.Core.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IValidator<LoginDto> _loginDtoValidator;
        private readonly IValidator<RegisterDto> _registerDtoValidator;
        private readonly IValidator<GrantUserRolesDto> _grantUserRolesDtoValidator;
        private readonly IPasswordHashService _passwordHashService;
        private readonly IRepository<User, string> _userRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly JwtOptions _jwtOptions;

        public AuthService(
            IValidator<LoginDto> loginDtoValidator,
            IValidator<RegisterDto> registerDtoValidator,
            IPasswordHashService passwordHashService,
            IRepository<User, string> userRepository,
            IOptions<JwtOptions> jwtOptions,
            IRepository<UserRole, long> userRoleRepository,
            IValidator<GrantUserRolesDto> grantUserRolesDtoValidator)
        {
            _loginDtoValidator = loginDtoValidator;
            _registerDtoValidator = registerDtoValidator;
            _passwordHashService = passwordHashService;
            _userRepository = userRepository;
            _jwtOptions = jwtOptions.Value;
            _userRoleRepository = userRoleRepository;
            _grantUserRolesDtoValidator = grantUserRolesDtoValidator;
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

        public async Task<ServiceResultDto> RegisterAsync(RegisterDto registerDto)
        {
            await _registerDtoValidator.ValidateAndThrowAsync(registerDto);

            var user = new User
            {
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                MiddleName = registerDto.MiddleName,
                PhoneNumber = registerDto.PhoneNumber,
                BirthDate = registerDto.BirthDate
            };

            (user.PasswordHash, user.PasswordSalt) = _passwordHashService.HashPassword(registerDto.Password);

            var userCreated = await _userRepository.InsertAsync(user);

            if (userCreated)
            {
                Log.Information("A new user account {userEmail} has been created.", user.Email);
            }
            else
            {
                return new ServiceResultDto
                {
                    IsSuccess = false,
                    Errors = ["User was not created."]
                };
            }

            var roleAddedForUser = await _userRoleRepository.InsertAsync(new UserRole
            {
                UserId = user.Id,
                RoleId = registerDto.RoleId
            });

            if (roleAddedForUser)
            {
                Log.Information("A user with ID {UserId} was granted a role {RoleId}.", user.Id, registerDto.RoleId);
            }

            return new ServiceResultDto
            {
                IsSuccess = roleAddedForUser,
                Errors = roleAddedForUser ? ["Failed to create a user and assign it to the role."] : []
            };
        }

        public async Task<ServiceResultDto> GrantUserRolesAsync(GrantUserRolesDto grantUserRolesDto)
        {
            await _grantUserRolesDtoValidator.ValidateAndThrowAsync(grantUserRolesDto);

            if (await CheckIfUserAlreadyHasRolesWhichShouldBeAddedAsync(grantUserRolesDto))
            {
                return new ServiceResultDto
                {
                    IsSuccess = false,
                    Errors = ["The user already has some of the roles."]
                };
            }

            var rolesGranted = await _userRoleRepository.InsertRangeAsync(
                [.. grantUserRolesDto.RoleIds
                    .Select(r => new UserRole { RoleId = r, UserId = grantUserRolesDto.UserId})]);

            if (rolesGranted)
            {
                var rolesList = JsonConvert.SerializeObject(grantUserRolesDto.RoleIds);
                Log.Information("User {UserId} has been granted role(s) {RolesList}.", grantUserRolesDto.UserId, rolesList);
            }

            return new ServiceResultDto
            {
                IsSuccess = rolesGranted,
                Errors = rolesGranted ? [] : ["Failed to grant roles."]
            };
        }

        private async Task<bool> CheckIfUserAlreadyHasRolesWhichShouldBeAddedAsync(GrantUserRolesDto grantUserRolesDto)
        {
            var existingUserRoleIds = await _userRoleRepository.DbSet
                .AsNoTracking()
                .Where(ur => ur.UserId == grantUserRolesDto.UserId)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            var alreadyExistingUserRoles = existingUserRoleIds
                .Intersect(grantUserRolesDto.RoleIds)
                .ToList();

            return alreadyExistingUserRoles.Count > 0;
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
                    ..user.Roles.Select(r => new Claim(ClaimTypes.Role, r.Name))
                ],
                expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiresInMinutes),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
