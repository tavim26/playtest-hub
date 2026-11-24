using FluentValidation;
using PlaytestHub.Modules.Studio.Application.Dtos;

namespace PlaytestHub.Modules.Studio.Application.Validators;

internal sealed class RejectCampaignRequestValidator : AbstractValidator<RejectCampaignRequest>
{
    public RejectCampaignRequestValidator()
    {
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(1000);
    }
}