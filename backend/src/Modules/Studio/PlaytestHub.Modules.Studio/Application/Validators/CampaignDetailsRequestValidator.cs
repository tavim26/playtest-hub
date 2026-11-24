using FluentValidation;
using PlaytestHub.Modules.Studio.Application.Dtos;

namespace PlaytestHub.Modules.Studio.Application.Validators;

internal sealed class CampaignDetailsRequestValidator : AbstractValidator<CampaignDetailsRequest>
{
    public CampaignDetailsRequestValidator()
    {
        RuleFor(x => x.BuildVersion).NotEmpty().MaximumLength(40);
        RuleFor(x => x.Description).NotNull().MaximumLength(2000);
        RuleFor(x => x.Seats).InclusiveBetween(1, 1000);
        RuleFor(x => x.MinimumReputation).GreaterThanOrEqualTo(0);

        RuleFor(x => x.EndsAt)
            .GreaterThan(x => x.StartsAt).WithMessage("EndsAt must be after StartsAt.")
            .Must(endsAt => endsAt > DateTimeOffset.UtcNow).WithMessage("EndsAt must be in the future.");

        RuleFor(x => x.Scenarios)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .Must(scenarios => scenarios.Count <= 20).WithMessage("A campaign can have at most 20 scenarios.");

        RuleForEach(x => x.Scenarios).ChildRules(scenario =>
        {
            scenario.RuleFor(s => s.Title).NotEmpty().MaximumLength(120);
            scenario.RuleFor(s => s.Description).NotNull().MaximumLength(1000);
        });
    }
}