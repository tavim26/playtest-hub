using FluentValidation;
using PlaytestHub.Modules.Identity.Application.Dtos;

namespace PlaytestHub.Modules.Identity.Application.Validators;

internal sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}