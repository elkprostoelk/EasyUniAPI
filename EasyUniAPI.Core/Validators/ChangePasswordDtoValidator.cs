using EasyUniAPI.Common.Dto;
using EasyUniAPI.DataAccess;
using EasyUniAPI.DataAccess.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace EasyUniAPI.Core.Validators
{
    public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
    {
        private readonly IRepository<User, string> _userRepository;

        public ChangePasswordDtoValidator(IRepository<User, string> userRepository)
        {
            _userRepository = userRepository;

            RuleFor(dto => dto.UserId)
                .NotEmpty()
                .MustAsync(DoesUserExistAsync)
                .WithMessage("User does not exist.");

            RuleFor(dto => dto.OldPassword)
                .NotEmpty()
                .Length(8, 30);

            RuleFor(dto => dto.NewPassword)
                .NotEmpty()
                .Length(8, 30)
                .NotEqual(dto => dto.OldPassword)
                .WithMessage("New password should differ from the previous one.");
        }

        private async Task<bool> DoesUserExistAsync(
            string userId,
            CancellationToken token) => await _userRepository.DbSet
                .AnyAsync(u => u.Id == userId, token);
    }
}
