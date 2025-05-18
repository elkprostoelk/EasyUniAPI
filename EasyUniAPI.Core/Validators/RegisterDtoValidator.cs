using EasyUniAPI.Common.Dto;
using EasyUniAPI.DataAccess;
using EasyUniAPI.DataAccess.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace EasyUniAPI.Core.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        private readonly IRepository<User, string> _userRepository;

        public RegisterDtoValidator(IRepository<User, string> userRepository)
        {
            _userRepository = userRepository;

            RuleFor(dto => dto.Gender)
                .NotNull()
                .IsInEnum();

            RuleFor(dto => dto.Email)
                .EmailAddress()
                .NotEmpty()
                .MaximumLength(256)
                .MustAsync(DoesUserNotExistAsync)
                .WithMessage("User with such email already exists.");

            RuleFor(dto => dto.Password)
                .NotEmpty()
                .Length(8, 30);

            RuleFor(dto => dto.RoleId)
                .GreaterThan(0);

            RuleFor(dto => dto.PhoneNumber)
                .NotEmpty()
                .Matches("^\\+?[1-9]\\d{1,14}$") // +xxxxxxxxxxxxxxx - 15 characters max
                .MaximumLength(20);

            RuleFor(dto => dto.BirthDate)
                .NotEmpty()
                .LessThan(DateOnly.FromDateTime(DateTime.Now))
                .WithMessage("Please enter an actual birth date.");

            RuleFor(dto => dto.FirstName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(dto => dto.LastName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(dto => dto.MiddleName)
                .MaximumLength(100);
        }

        private async Task<bool> DoesUserNotExistAsync(string email, CancellationToken token)
        {
            return !await _userRepository.DbSet
                .AnyAsync(u => u.Email == email, token);
        }
    }
}
