using PlaytestHub.Shared.Kernel;

namespace PlaytestHub.Modules.Studio.Domain;

internal static class CampaignErrors
{
    public static readonly Error NotFound =
        Error.NotFound("Campaigns.NotFound", "The campaign was not found.");

    public static readonly Error NotEditable =
        Error.Conflict("Campaigns.NotEditable", "Only draft or rejected campaigns can be edited.");

    public static readonly Error NoScenarios =
        Error.Validation("Campaigns.NoScenarios", "Add at least one test scenario before submitting for review.");

    public static readonly Error AlreadyEnded =
        Error.Validation("Campaigns.AlreadyEnded", "The campaign end date is in the past.");

    public static Error InvalidTransition(CampaignStatus current, string action) =>
        Error.Conflict("Campaigns.InvalidTransition", $"A campaign that is {current} cannot be {action}.");
}