using FluentValidation;
using PlaytestHub.Modules.Identity.Application.Dtos;

namespace PlaytestHub.Modules.Identity.Application.Validators;

internal sealed class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}