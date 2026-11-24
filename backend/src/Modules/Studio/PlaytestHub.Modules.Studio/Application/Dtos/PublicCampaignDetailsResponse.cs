using PlaytestHub.Modules.Studio.Domain;

namespace PlaytestHub.Modules.Studio.Application.Dtos;

internal sealed record PublicCampaignDetailsResponse(
    Guid Id,
    Guid GameId,
    string GameTitle,
    string GameDescription,
    SubGenre Genre,
    IReadOnlyList<Platforms> Platforms,
    string? MinimumSpecs,
    string BuildVersion,
    string Description,
    int Seats,
    int MinimumReputation,
    DateTime StartsAtUtc,
    DateTime EndsAtUtc,
    IReadOnlyList<ScenarioResponse> Scenarios)
{
    public static PublicCampaignDetailsResponse From(Campaign campaign, Game game) => new(
        campaign.Id,
        game.Id,
        game.Title,
        game.Description,
        game.Genre,
        PlatformMapping.ToList(game.Platforms),
        game.MinimumSpecs,
        campaign.BuildVersion,
        campaign.Description,
        campaign.Seats,
        campaign.MinimumReputation,
        campaign.StartsAtUtc,
        campaign.EndsAtUtc,
        campaign.Scenarios
            .OrderBy(s => s.Position)
            .Select(s => new ScenarioResponse(s.Id, s.Title, s.Description))
            .ToList());
}