using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PlaytestHub.Modules.Studio.Application.Dtos;
using PlaytestHub.Modules.Studio.Application.Services;
using PlaytestHub.Modules.Studio.Domain;
using PlaytestHub.Shared.Infrastructure.Errors;
using PlaytestHub.Shared.Kernel;

namespace PlaytestHub.Modules.Studio.Endpoints;

internal static class PublicCampaignEndpoints
{
    public static void MapPublicCampaignEndpoints(this IEndpointRouteBuilder group)
    {
        var campaigns = group.MapGroup("/public/campaigns").AllowAnonymous();

        campaigns.MapGet(string.Empty, SearchAsync)
            .Produces<PagedResponse<CampaignListItemResponse>>();

        campaigns.MapGet("/{campaignId:guid}", GetAsync)
            .Produces<PublicCampaignDetailsResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> SearchAsync(
        ICampaignCatalog catalog,
        SubGenre? genre,
        Platforms? platform,
        string? search,
        int page = 1,
        int pageSize = 20)
    {
        var result = await catalog.SearchAsync(new CampaignSearch(genre, platform, search, page, pageSize));

        return Results.Ok(result);
    }

    private static async Task<IResult> GetAsync(Guid campaignId, ICampaignCatalog catalog)
    {
        var result = await catalog.GetPublishedAsync(campaignId);

        return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
    }
}