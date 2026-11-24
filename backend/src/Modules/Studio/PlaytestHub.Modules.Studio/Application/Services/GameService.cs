using Microsoft.EntityFrameworkCore;
using PlaytestHub.Modules.Identity.Contracts;
using PlaytestHub.Modules.Studio.Application.Dtos;
using PlaytestHub.Modules.Studio.Domain;
using PlaytestHub.Modules.Studio.Infrastructure.Database;
using PlaytestHub.Shared.Kernel;

namespace PlaytestHub.Modules.Studio.Application.Services;

internal sealed class GameService(StudioDbContext dbContext, IUserLookup userLookup) : IGameService
{
    public async Task<GameResponse> CreateAsync(GameDetailsRequest request, Guid currentUserId)
    {
        var game = Game.Create(
            request.Title,
            request.Description,
            request.Genre,
            PlatformMapping.ToFlags(request.Platforms),
            request.MinimumSpecs,
            currentUserId);

        dbContext.Games.Add(game);
        await dbContext.SaveChangesAsync();

        return await ToResponseAsync(game);
    }

    public async Task<Result<GameResponse>> GetByIdAsync(Guid gameId)
    {
        var game = await FindGameAsync(gameId);

        return game is null ? GameErrors.NotFound : await ToResponseAsync(game);
    }

    public async Task<IReadOnlyList<GameResponse>> GetMineAsync(Guid currentUserId)
    {
        var games = await dbContext.Games
            .Include(g => g.Members)
            .Where(g => g.Members.Any(m => m.UserId == currentUserId))
            .OrderByDescending(g => g.CreatedAtUtc)
            .ToListAsync();

        return await ToResponsesAsync(games);
    }

    public async Task<Result<GameResponse>> UpdateAsync(Guid gameId, GameDetailsRequest request, Guid currentUserId)
    {
        var game = await FindGameAsync(gameId);

        if (game is null)
        {
            return GameErrors.NotFound;
        }

        if (!game.IsOwner(currentUserId))
        {
            return GameErrors.NotOwner;
        }

        game.UpdateDetails(
            request.Title,
            request.Description,
            request.Genre,
            PlatformMapping.ToFlags(request.Platforms),
            request.MinimumSpecs);

        await dbContext.SaveChangesAsync();

        return await ToResponseAsync(game);
    }

    public async Task<Result<GameResponse>> AddCollaboratorAsync(
        Guid gameId,
        AddCollaboratorRequest request,
        Guid currentUserId)
    {
        var game = await FindGameAsync(gameId);

        if (game is null)
        {
            return GameErrors.NotFound;
        }

        if (!game.IsOwner(currentUserId))
        {
            return GameErrors.NotOwner;
        }

        var user = await userLookup.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return GameErrors.UserNotFound;
        }

        var result = game.AddCollaborator(user.Id);

        if (result.IsFailure)
        {
            return result.Error;
        }

        await dbContext.SaveChangesAsync();

        return await ToResponseAsync(game);
    }

    public async Task<Result> RemoveCollaboratorAsync(Guid gameId, Guid userId, Guid currentUserId)
    {
        var game = await FindGameAsync(gameId);

        if (game is null)
        {
            return Result.Failure(GameErrors.NotFound);
        }

        if (!game.IsOwner(currentUserId))
        {
            return Result.Failure(GameErrors.NotOwner);
        }

        var result = game.RemoveCollaborator(userId);

        if (result.IsFailure)
        {
            return result;
        }

        await dbContext.SaveChangesAsync();

        return Result.Success();
    }

    private Task<Game?> FindGameAsync(Guid gameId) =>
        dbContext.Games
            .Include(g => g.Members)
            .SingleOrDefaultAsync(g => g.Id == gameId);

    private async Task<GameResponse> ToResponseAsync(Game game) =>
        (await ToResponsesAsync([game]))[0];

    private async Task<IReadOnlyList<GameResponse>> ToResponsesAsync(IReadOnlyCollection<Game> games)
    {
        var users = await userLookup.GetByIdsAsync(games.SelectMany(g => g.Members).Select(m => m.UserId));

        return games.Select(game => GameResponse.FromGame(game, users)).ToList();
    }
}