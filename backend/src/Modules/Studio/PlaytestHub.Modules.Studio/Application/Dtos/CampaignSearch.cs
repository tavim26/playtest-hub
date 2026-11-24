using PlaytestHub.Modules.Studio.Domain;

namespace PlaytestHub.Modules.Studio.Application.Dtos;

internal sealed record CampaignSearch(SubGenre? Genre, Platforms? Platform, string? Text, int Page, int PageSize);