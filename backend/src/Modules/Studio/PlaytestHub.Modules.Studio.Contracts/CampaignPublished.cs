using PlaytestHub.Shared.Kernel;

namespace PlaytestHub.Modules.Studio.Contracts;

public sealed record CampaignPublished(Guid CampaignId, Guid GameId, DateTime PublishedAtUtc) : IDomainEvent;