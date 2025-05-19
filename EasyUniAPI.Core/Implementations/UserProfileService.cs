using EasyUniAPI.Common.Dto;
using EasyUniAPI.Core.Interfaces;
using EasyUniAPI.Core.Mappers;
using EasyUniAPI.DataAccess;
using EasyUniAPI.DataAccess.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace EasyUniAPI.Core.Implementations
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IRepository<User, string> _userRepository;
        private readonly IValidator<UpdateUserProfileDto> _updateUserProfileValidator;

        public UserProfileService(
            IRepository<User, string> userRepository,
            IValidator<UpdateUserProfileDto> updateUserProfileValidator)
        {
            _userRepository = userRepository;
            _updateUserProfileValidator = updateUserProfileValidator;
        }

        public async Task<ServiceResultDto<UserProfileDto>> GetUserProfileAsync(string userId)
        {
            var user = await _userRepository.DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null)
            {
                return new ServiceResultDto<UserProfileDto>
                {
                    IsSuccess = false,
                    Errors = ["User not found."]
                };
            }

            return new ServiceResultDto<UserProfileDto>
            {
                IsSuccess = true,
                Result = user.MapUserToUserProfileDto()
            };
        }

        public async Task<ServiceResultDto> UpdateUserProfileAsync(string userId, UpdateUserProfileDto updateUserProfileDto)
        {
            var user = await _userRepository.DbSet
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user is null)
            {
                return new ServiceResultDto
                {
                    IsSuccess = false,
                    Errors = ["User not found."]
                };
            }

            await _updateUserProfileValidator.ValidateAndThrowAsync(updateUserProfileDto);

            user.Email = updateUserProfileDto.Email;
            user.FirstName = updateUserProfileDto.FirstName;
            user.LastName = updateUserProfileDto.LastName;
            user.MiddleName = updateUserProfileDto.MiddleName;
            user.PhoneNumber = updateUserProfileDto.PhoneNumber;
            user.BirthDate = updateUserProfileDto.BirthDate;
            user.Gender = updateUserProfileDto.Gender;

            var updated = await _userRepository.UpdateAsync(user);
            return new ServiceResultDto
            {
                IsSuccess = updated,
                Errors = updated ? [] : ["Failed to update a user profile."]
            };
        }
    }
}
