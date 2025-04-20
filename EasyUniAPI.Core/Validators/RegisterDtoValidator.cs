using EasyUniAPI.Common.Dto;
using FluentValidation;

namespace EasyUniAPI.Core.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(dto => dto.Email)
                .EmailAddress()
                .NotEmpty()
                .MaximumLength(256);

            RuleFor(dto => dto.Password)
                .NotEmpty()
                .Length(8, 20);

            RuleFor(dto => dto.RoleId)
                .GreaterThan(0);

            RuleFor(dto => dto.PhoneNumber)
                .NotEmpty()
                .Matches("^\\+?[1-9]\\d{1,14}$") // +xxxxxxxxxxxxxxx - 15 characters max
                .MaximumLength(20);

            RuleFor(dto => dto.BirthDate)
                .NotEmpty()
                .LessThan(DateOnly.FromDateTime(DateTime.Now));

            RuleFor(dto => dto.FirstName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(dto => dto.LastName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(dto => dto.MiddleName)
                .MaximumLength(100);
        }
    }
}
