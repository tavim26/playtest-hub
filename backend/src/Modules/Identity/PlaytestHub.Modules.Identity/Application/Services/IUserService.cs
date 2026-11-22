using PlaytestHub.Modules.Identity.Application.Dtos;
using PlaytestHub.Shared.Kernel;

namespace PlaytestHub.Modules.Identity.Application.Services;

internal interface IUserService
{
    Task<Result<UserResponse>> RegisterAsync(RegisterRequest request);
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
    Task<Result<AuthResponse>> RefreshAsync(RefreshTokenRequest request);
    Task LogoutAsync(RefreshTokenRequest request);
    Task<Result<UserResponse>> GetByIdAsync(Guid userId);
}