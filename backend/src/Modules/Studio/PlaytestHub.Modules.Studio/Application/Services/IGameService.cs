using PlaytestHub.Modules.Studio.Application.Dtos;
using PlaytestHub.Shared.Kernel;

namespace PlaytestHub.Modules.Studio.Application.Services;

internal interface IGameService
{
    Task<GameResponse> CreateAsync(GameDetailsRequest request, Guid currentUserId);
    Task<Result<GameResponse>> GetByIdAsync(Guid gameId);
    Task<IReadOnlyList<GameResponse>> GetMineAsync(Guid currentUserId);
    Task<Result<GameResponse>> UpdateAsync(Guid gameId, GameDetailsRequest request, Guid currentUserId);
    Task<Result<GameResponse>> AddCollaboratorAsync(Guid gameId, AddCollaboratorRequest request, Guid currentUserId);
    Task<Result> RemoveCollaboratorAsync(Guid gameId, Guid userId, Guid currentUserId);
}