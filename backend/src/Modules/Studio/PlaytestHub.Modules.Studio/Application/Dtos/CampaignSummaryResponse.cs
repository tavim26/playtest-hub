using PlaytestHub.Modules.Studio.Domain;

namespace PlaytestHub.Modules.Studio.Application.Dtos;

internal sealed record CampaignSummaryResponse(
    Guid Id,
    Guid GameId,
    string BuildVersion,
    int Seats,
    DateTime StartsAtUtc,
    DateTime EndsAtUtc,
    CampaignStatus Status);