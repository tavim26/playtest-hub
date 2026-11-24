namespace PlaytestHub.Modules.Studio.Application.Services;

// Read model exposed to other modules 
public sealed record CampaignInfo(
    Guid CampaignId,
    Guid GameId,
    Guid OwnerId,
    int Seats,
    int MinimumReputation,
    DateTime StartsAtUtc,
    DateTime EndsAtUtc,
    bool IsOpen);

public interface ICampaignReader
{
    Task<CampaignInfo?> GetByIdAsync(Guid campaignId, CancellationToken cancellationToken = default);
}