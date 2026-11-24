using PlaytestHub.Modules.Studio.Domain;

namespace PlaytestHub.Modules.Studio.Application.Dtos;

internal sealed record CampaignListItemResponse(
    Guid Id,
    Guid GameId,
    string GameTitle,
    SubGenre Genre,
    IReadOnlyList<Platforms> Platforms,
    string BuildVersion,
    int Seats,
    int MinimumReputation,
    DateTime StartsAtUtc,
    DateTime EndsAtUtc)
{
    public static CampaignListItemResponse From(Campaign campaign, Game game) => new(
        campaign.Id,
        game.Id,
        game.Title,
        game.Genre,
        PlatformMapping.ToList(game.Platforms),
        campaign.BuildVersion,
        campaign.Seats,
        campaign.MinimumReputation,
        campaign.StartsAtUtc,
        campaign.EndsAtUtc);
}