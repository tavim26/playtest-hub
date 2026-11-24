using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PlaytestHub.Modules.Identity.Contracts;
using PlaytestHub.Modules.Studio.Application.Dtos;
using PlaytestHub.Modules.Studio.Application.Services;
using PlaytestHub.Shared.Infrastructure.Errors;
using PlaytestHub.Shared.Infrastructure.Validation;

namespace PlaytestHub.Modules.Studio.Endpoints;

internal static class ModerationEndpoints
{
    public static void MapModerationEndpoints(this IEndpointRouteBuilder group)
    {
        // Authorize only for the Moderator role
        var moderation = group.MapGroup("/moderation/campaigns")
            .RequireAuthorization(policy => policy.RequireRole(Roles.Moderator));

        moderation.MapPost("/{campaignId:guid}/approve", ApproveAsync)
            .Produces<CampaignResponse>()
            .ProducesProblem(StatusCodes.Status409Conflict);

        moderation.MapPost("/{campaignId:guid}/reject", RejectAsync)
            .WithRequestValidation<RejectCampaignRequest>()
            .Produces<CampaignResponse>()
            .ProducesProblem(StatusCodes.Status409Conflict);
        
        moderation.MapGet("/pending", GetPendingAsync)
            .Produces<IReadOnlyList<CampaignListItemResponse>>();
    }

    private static async Task<IResult> ApproveAsync(Guid campaignId, ICampaignService service)
    {
        var result = await service.ApproveAsync(campaignId);

        return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
    }

    private static async Task<IResult> RejectAsync(
        Guid campaignId,
        RejectCampaignRequest request,
        ICampaignService service)
    {
        var result = await service.RejectAsync(campaignId, request);

        return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
    }
    
    private static async Task<IResult> GetPendingAsync(ICampaignCatalog catalog) =>
        Results.Ok(await catalog.GetPendingAsync());
}