using PlaytestHub.Modules.Studio.Application.Dtos;
using PlaytestHub.Shared.Kernel;

namespace PlaytestHub.Modules.Studio.Application.Services;

internal interface ICampaignService
{
    Task<Result<CampaignResponse>> CreateAsync(Guid gameId, CampaignDetailsRequest request, Guid userId);
    Task<Result<IReadOnlyList<CampaignSummaryResponse>>> GetByGameAsync(Guid gameId, Guid userId);
    Task<Result<CampaignResponse>> GetByIdAsync(Guid campaignId, Guid userId, bool isModerator);
    Task<Result<CampaignResponse>> UpdateAsync(Guid campaignId, CampaignDetailsRequest request, Guid userId);
    Task<Result<CampaignResponse>> SubmitAsync(Guid campaignId, Guid userId);
    Task<Result<CampaignResponse>> CloseAsync(Guid campaignId, Guid userId);
    Task<Result<CampaignResponse>> ApproveAsync(Guid campaignId);
    Task<Result<CampaignResponse>> RejectAsync(Guid campaignId, RejectCampaignRequest request);
}