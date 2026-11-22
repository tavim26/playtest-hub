using PlaytestHub.Modules.Identity.Application.Dtos;
using PlaytestHub.Shared.Kernel;

namespace PlaytestHub.Modules.Identity.Application.Services;

internal interface IUserService
{
    Task<Result<UserResponse>> RegisterAsync(RegisterRequest request);
    Task<Result<LoginResponse>> LoginAsync(LoginRequest request);
    Task<Result<UserResponse>> GetByIdAsync(Guid userId);
}