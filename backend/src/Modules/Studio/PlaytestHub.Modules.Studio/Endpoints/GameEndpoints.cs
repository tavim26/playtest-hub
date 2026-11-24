using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PlaytestHub.Modules.Studio.Application.Dtos;
using PlaytestHub.Modules.Studio.Application.Services;
using PlaytestHub.Shared.Infrastructure.Authentication;
using PlaytestHub.Shared.Infrastructure.Errors;
using PlaytestHub.Shared.Infrastructure.Validation;

namespace PlaytestHub.Modules.Studio.Endpoints;

internal static class GameEndpoints
{
    public static void MapGameEndpoints(this IEndpointRouteBuilder group)
    {
        var games = group.MapGroup("/games");

        games.MapPost("", CreateAsync)
            .WithRequestValidation<GameDetailsRequest>()
            .Produces<GameResponse>(StatusCodes.Status201Created);

        games.MapGet("/mine", GetMineAsync)
            .Produces<IReadOnlyList<GameResponse>>();

        games.MapGet("/{gameId:guid}", GetByIdAsync)
            .Produces<GameResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        games.MapPut("/{gameId:guid}", UpdateAsync)
            .WithRequestValidation<GameDetailsRequest>()
            .Produces<GameResponse>()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        games.MapPost("/{gameId:guid}/collaborators", AddCollaboratorAsync)
            .WithRequestValidation<AddCollaboratorRequest>()
            .Produces<GameResponse>()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        games.MapDelete("/{gameId:guid}/collaborators/{userId:guid}", RemoveCollaboratorAsync)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> CreateAsync(
        GameDetailsRequest request,
        ClaimsPrincipal principal,
        IGameService gameService)
    {
        var game = await gameService.CreateAsync(request, principal.GetUserId());

        return Results.Created($"/api/studio/games/{game.Id}", game);
    }

    private static async Task<IResult> GetMineAsync(ClaimsPrincipal principal, IGameService gameService) =>
        Results.Ok(await gameService.GetMineAsync(principal.GetUserId()));

    private static async Task<IResult> GetByIdAsync(Guid gameId, IGameService gameService)
    {
        var result = await gameService.GetByIdAsync(gameId);

        return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
    }

    private static async Task<IResult> UpdateAsync(
        Guid gameId,
        GameDetailsRequest request,
        ClaimsPrincipal principal,
        IGameService gameService)
    {
        var result = await gameService.UpdateAsync(gameId, request, principal.GetUserId());

        return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
    }

    private static async Task<IResult> AddCollaboratorAsync(
        Guid gameId,
        AddCollaboratorRequest request,
        ClaimsPrincipal principal,
        IGameService gameService)
    {
        var result = await gameService.AddCollaboratorAsync(gameId, request, principal.GetUserId());

        return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
    }

    private static async Task<IResult> RemoveCollaboratorAsync(
        Guid gameId,
        Guid userId,
        ClaimsPrincipal principal,
        IGameService gameService)
    {
        var result = await gameService.RemoveCollaboratorAsync(gameId, userId, principal.GetUserId());

        return result.IsSuccess ? Results.NoContent() : result.ToProblem();
    }
}