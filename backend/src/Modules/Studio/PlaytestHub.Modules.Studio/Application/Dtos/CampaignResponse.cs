using PlaytestHub.Modules.Studio.Domain;

namespace PlaytestHub.Modules.Studio.Application.Dtos;

internal sealed record ScenarioResponse(Guid Id, string Title, string Description);

internal sealed record CampaignResponse(
    Guid Id,
    Guid GameId,
    string BuildVersion,
    string Description,
    int Seats,
    int MinimumReputation,
    DateTime StartsAtUtc,
    DateTime EndsAtUtc,
    CampaignStatus Status,
    string? RejectionReason,
    DateTime CreatedAtUtc,
    DateTime? PublishedAtUtc,
    IReadOnlyList<ScenarioResponse> Scenarios)
{
    public static CampaignResponse FromCampaign(Campaign campaign) => new(
        campaign.Id,
        campaign.GameId,
        campaign.BuildVersion,
        campaign.Description,
        campaign.Seats,
        campaign.MinimumReputation,
        campaign.StartsAtUtc,
        campaign.EndsAtUtc,
        campaign.Status,
        campaign.RejectionReason,
        campaign.CreatedAtUtc,
        campaign.PublishedAtUtc,
        campaign.Scenarios
            .OrderBy(s => s.Position)
            .Select(s => new ScenarioResponse(s.Id, s.Title, s.Description))
            .ToList());
}