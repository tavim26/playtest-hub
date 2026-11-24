using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PlaytestHub.Modules.Identity.Contracts;
using PlaytestHub.Modules.Studio.Application.Dtos;
using PlaytestHub.Modules.Studio.Application.Services;
using PlaytestHub.Shared.Infrastructure.Authentication;
using PlaytestHub.Shared.Infrastructure.Errors;
using PlaytestHub.Shared.Infrastructure.Validation;

namespace PlaytestHub.Modules.Studio.Endpoints;

internal static class CampaignEndpoints
{
    public static void MapCampaignEndpoints(this IEndpointRouteBuilder group)
    {
        group.MapPost("/games/{gameId:guid}/campaigns", CreateAsync)
            .WithRequestValidation<CampaignDetailsRequest>()
            .Produces<CampaignResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/games/{gameId:guid}/campaigns", GetByGameAsync)
            .Produces<IReadOnlyList<CampaignSummaryResponse>>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/campaigns/{campaignId:guid}", GetByIdAsync)
            .Produces<CampaignResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/campaigns/{campaignId:guid}", UpdateAsync)
            .WithRequestValidation<CampaignDetailsRequest>()
            .Produces<CampaignResponse>()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/campaigns/{campaignId:guid}/submit", SubmitAsync)
            .Produces<CampaignResponse>()
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/campaigns/{campaignId:guid}/close", CloseAsync)
            .Produces<CampaignResponse>()
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> CreateAsync(
        Guid gameId,
        CampaignDetailsRequest request,
        ClaimsPrincipal principal,
        ICampaignService service)
    {
        var result = await service.CreateAsync(gameId, request, principal.GetUserId());

        return result.IsSuccess
            ? Results.Created($"/api/studio/campaigns/{result.Value.Id}", result.Value)
            : result.ToProblem();
    }

    private static async Task<IResult> GetByGameAsync(Guid gameId, ClaimsPrincipal principal, ICampaignService service)
    {
        var result = await service.GetByGameAsync(gameId, principal.GetUserId());

        return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
    }

    private static async Task<IResult> GetByIdAsync(Guid campaignId, ClaimsPrincipal principal, ICampaignService service)
    {
        var result = await service.GetByIdAsync(
            campaignId,
            principal.GetUserId(),
            principal.IsInRole(Roles.Moderator));

        return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
    }

    private static async Task<IResult> UpdateAsync(
        Guid campaignId,
        CampaignDetailsRequest request,
        ClaimsPrincipal principal,
        ICampaignService service)
    {
        var result = await service.UpdateAsync(campaignId, request, principal.GetUserId());

        return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
    }

    private static async Task<IResult> SubmitAsync(Guid campaignId, ClaimsPrincipal principal, ICampaignService service)
    {
        var result = await service.SubmitAsync(campaignId, principal.GetUserId());

        return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
    }

    private static async Task<IResult> CloseAsync(Guid campaignId, ClaimsPrincipal principal, ICampaignService service)
    {
        var result = await service.CloseAsync(campaignId, principal.GetUserId());

        return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
    }
}