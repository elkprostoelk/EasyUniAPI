using EasyUniAPI.Common.Configurations;
using EasyUniAPI.Common.Dto;
using EasyUniAPI.Core.Interfaces;
using EasyUniAPI.Core.Mappers;
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
        private readonly IValidator<ChangePasswordDto> _changePasswordDtoValidator;
        private readonly IPasswordHashService _passwordHashService;
        private readonly IRepository<User, string> _userRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly JwtOptions _jwtOptions;
        private readonly IClaimsProvider _claimsProvider;

        private const int MaxFailedLoginAttempts = 5;

        public AuthService(
            IValidator<LoginDto> loginDtoValidator,
            IValidator<RegisterDto> registerDtoValidator,
            IPasswordHashService passwordHashService,
            IRepository<User, string> userRepository,
            IOptions<JwtOptions> jwtOptions,
            IRepository<UserRole, long> userRoleRepository,
            IValidator<GrantUserRolesDto> grantUserRolesDtoValidator,
            IValidator<ChangePasswordDto> changePasswordDtoValidator,
            IClaimsProvider claimsProvider)
        {
            _loginDtoValidator = loginDtoValidator;
            _registerDtoValidator = registerDtoValidator;
            _passwordHashService = passwordHashService;
            _userRepository = userRepository;
            _jwtOptions = jwtOptions.Value;
            _userRoleRepository = userRoleRepository;
            _grantUserRolesDtoValidator = grantUserRolesDtoValidator;
            _changePasswordDtoValidator = changePasswordDtoValidator;
            _claimsProvider = claimsProvider;
        }

        public async Task<ServiceResultDto<string>> LoginAsync(LoginDto loginDto)
        {
            await _loginDtoValidator.ValidateAndThrowAsync(loginDto);

            var user = await _userRepository.DbSet
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

            if (!user.Active)
            {
                return new ServiceResultDto<string>
                {
                    IsSuccess = false,
                    Errors = ["User is blocked. Please contact the administrator team."]
                };
            }

            var (hash, _) = _passwordHashService.HashPassword(loginDto.Password, user.PasswordSalt);
            var isPasswordValid = user.PasswordHash == hash;

            if (!isPasswordValid)
            {
                ++user.FailedLoginAttempts;
                if (user.FailedLoginAttempts == MaxFailedLoginAttempts)
                {
                    user.Active = false;
                    Log.Information("User {userEmail} was blocked after {failedAttempts} failed login attempts.", user.Email, user.FailedLoginAttempts);
                }

                await _userRepository.UpdateAsync(user);
            }

            return new ServiceResultDto<string>
            {
                IsSuccess = isPasswordValid,
                Errors = isPasswordValid ? []
                    : [$"Invalid password. You have {MaxFailedLoginAttempts - user.FailedLoginAttempts} login attempt(s) left."],
                Result = isPasswordValid ? GenerateToken(user) : null
            };
        }

        public async Task<ServiceResultDto> UnlockUserAsync(string userId)
        {
            var loggedInUserRoles = _claimsProvider.GetLoggedInUserRoles();
            if (!loggedInUserRoles.Contains("Administrator", StringComparer.OrdinalIgnoreCase))
            {
                return new ServiceResultDto
                {
                    IsSuccess = false,
                    Errors = ["You do not have permission to unlock users."]
                };
            }

            var isUserIdValid = await ValidateUserIdAsync(userId);
            if (!isUserIdValid)
            {
                return new ServiceResultDto
                {
                    IsSuccess = false,
                    Errors = ["User ID is not valid."]
                };
            }

            var user = await _userRepository.DbSet
                .FirstAsync(u => u.Id == userId);

            if (user.Active)
            {
                return new ServiceResultDto
                {
                    IsSuccess = false,
                    Errors = ["User is already active."]
                };
            }

            user.Active = true;
            user.FailedLoginAttempts = 0;

            var userUpdated = await _userRepository.UpdateAsync(user);
            if (userUpdated)
            {
                Log.Information("User {userEmail} has been unlocked.", user.Email);
            }
            return new ServiceResultDto
            {
                IsSuccess = userUpdated,
                Errors = userUpdated ? [] : ["Failed to unlock the user."]
            };
        }

        public async Task<ServiceResultDto> RegisterAsync(RegisterDto registerDto)
        {
            await _registerDtoValidator.ValidateAndThrowAsync(registerDto);

            var user = registerDto.MapRegisterDtoToUser();

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
                Errors = roleAddedForUser ? [] : ["Failed to create a user and assign it to the role."]
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

        public async Task<ServiceResultDto> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            await _changePasswordDtoValidator.ValidateAndThrowAsync(changePasswordDto);

            var user = await _userRepository.DbSet
                .FirstAsync(u => u.Id == changePasswordDto.UserId);

            var (hash, _) = _passwordHashService.HashPassword(changePasswordDto.OldPassword, user.PasswordSalt);
            if (user.PasswordHash != hash)
            {
                return new ServiceResultDto
                {
                    IsSuccess = false,
                    Errors = ["Invalid old password."]
                };
            }

            (user.PasswordHash, _) = _passwordHashService.HashPassword(changePasswordDto.NewPassword, user.PasswordSalt);

            var passwordChanged = await _userRepository.UpdateAsync(user);
            if (passwordChanged)
            {
                Log.Information("Password has been updated for the user {UserId}.", changePasswordDto.UserId);
            }

            return new ServiceResultDto
            {
                IsSuccess = passwordChanged,
                Errors = passwordChanged ? [] : ["Failed to change a password."]
            };
        }

        #region Private methods

        private async Task<bool> ValidateUserIdAsync(string userId)
        {
            return !string.IsNullOrWhiteSpace(userId)
                && await _userRepository.DbSet
                       .AsNoTracking()
                       .AnyAsync(u => u.Id == userId);
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

        #endregion
    }
}
