using PlaytestHub.Modules.Studio.Application.Dtos;
using PlaytestHub.Shared.Kernel;

namespace PlaytestHub.Modules.Studio.Application.Services;

internal interface ICampaignCatalog
{
    Task<PagedResponse<CampaignListItemResponse>> SearchAsync(CampaignSearch search);
    Task<Result<PublicCampaignDetailsResponse>> GetPublishedAsync(Guid campaignId);
    Task<IReadOnlyList<CampaignListItemResponse>> GetPendingAsync();
}