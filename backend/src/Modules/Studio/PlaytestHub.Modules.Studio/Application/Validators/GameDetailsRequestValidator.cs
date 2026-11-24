using FluentValidation;
using PlaytestHub.Modules.Studio.Application.Dtos;
using PlaytestHub.Modules.Studio.Domain;

namespace PlaytestHub.Modules.Studio.Application.Validators;

internal sealed class GameDetailsRequestValidator : AbstractValidator<GameDetailsRequest>
{
    public GameDetailsRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Description).NotNull().MaximumLength(2000);
        RuleFor(x => x.Genre).IsInEnum();
        RuleFor(x => x.Platforms).NotEmpty();
        RuleForEach(x => x.Platforms).IsInEnum().NotEqual(Platforms.None);
        RuleFor(x => x.MinimumSpecs).MaximumLength(1000);
    }
}