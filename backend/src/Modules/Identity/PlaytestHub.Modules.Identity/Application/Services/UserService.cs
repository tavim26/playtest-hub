using Microsoft.AspNetCore.Identity;
using PlaytestHub.Modules.Identity.Application.Dtos;
using PlaytestHub.Modules.Identity.Application.Errors;
using PlaytestHub.Modules.Identity.Domain;
using PlaytestHub.Shared.Kernel;

namespace PlaytestHub.Modules.Identity.Application.Services;

internal sealed class UserService(UserManager<User> userManager, ITokenService tokenService) : IUserService
{
    public async Task<Result<UserResponse>> RegisterAsync(RegisterRequest request)
    {
        if (await userManager.FindByEmailAsync(request.Email) is not null)
        {
            return UserErrors.EmailAlreadyInUse;
        }

        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            DisplayName = request.DisplayName
        };

        // UserManager hashes the password and enforces the Identity password rules
        var identityResult = await userManager.CreateAsync(user, request.Password);

        if (!identityResult.Succeeded)
        {
            var description = string.Join(" ", identityResult.Errors.Select(e => e.Description));
            return UserErrors.CreationFailed(description);
        }

        return UserResponse.FromUser(user);
    }

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return UserErrors.InvalidCredentials;
        }

        var roles = await userManager.GetRolesAsync(user);
        var accessToken = tokenService.CreateAccessToken(user, roles);

        return new LoginResponse(accessToken.Token, accessToken.ExpiresAtUtc);
    }

    public async Task<Result<UserResponse>> GetByIdAsync(Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        return user is null
            ? UserErrors.NotFound
            : UserResponse.FromUser(user);
    }
}