namespace PlaytestHub.Modules.Studio.Application.Dtos;

internal sealed record ScenarioRequest(string Title, string Description);

internal sealed record CampaignDetailsRequest(
    string BuildVersion,
    string Description,
    int Seats,
    int MinimumReputation,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    IReadOnlyList<ScenarioRequest> Scenarios);