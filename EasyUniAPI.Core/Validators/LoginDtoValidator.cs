using EasyUniAPI.Common.Dto;
using FluentValidation;

namespace EasyUniAPI.Core.Validators
{
    public sealed class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(dto => dto.Login)
                .NotEmpty()
                .MaximumLength(256);

            RuleFor(dto => dto.Password)
                .NotEmpty()
                .Length(8, 30);
        }
    }
}
