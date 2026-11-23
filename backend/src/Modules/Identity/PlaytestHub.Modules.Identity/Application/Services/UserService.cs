using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlaytestHub.Modules.Identity.Application.Dtos;
using PlaytestHub.Modules.Identity.Application.Errors;
using PlaytestHub.Modules.Identity.Domain;
using PlaytestHub.Modules.Identity.Infrastructure.Database;
using PlaytestHub.Shared.Kernel;

namespace PlaytestHub.Modules.Identity.Application.Services;

internal sealed class UserService(
    UserManager<User> userManager,
    ITokenService tokenService,
    IdentityModuleDbContext dbContext) : IUserService
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

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return UserErrors.InvalidCredentials;
        }

        return await IssueTokensAsync(user);
    }

    public async Task<Result<AuthResponse>> RefreshAsync(RefreshTokenRequest request)
    {
        var hash = tokenService.HashRefreshToken(request.RefreshToken);
        var storedToken = await dbContext.RefreshTokens.SingleOrDefaultAsync(t => t.TokenHash == hash);

        if (storedToken is null)
        {
            return UserErrors.InvalidRefreshToken;
        }

        if (storedToken.RevokedAtUtc is not null)
        {
            // A rotated token was used again: assume it was stolen and end all sessions of this user
            await dbContext.RefreshTokens
                .Where(t => t.UserId == storedToken.UserId && t.RevokedAtUtc == null)
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.RevokedAtUtc, (DateTime?)DateTime.UtcNow));

            return UserErrors.InvalidRefreshToken;
        }

        if (!storedToken.IsActive)
        {
            return UserErrors.InvalidRefreshToken;
        }

        var user = await userManager.FindByIdAsync(storedToken.UserId.ToString());

        if (user is null)
        {
            return UserErrors.InvalidRefreshToken;
        }

        storedToken.Revoke();

        // Revoking the old token and storing the new one are saved together
        return await IssueTokensAsync(user);
    }

    public async Task LogoutAsync(RefreshTokenRequest request)
    {
        var hash = tokenService.HashRefreshToken(request.RefreshToken);
        var storedToken = await dbContext.RefreshTokens.SingleOrDefaultAsync(t => t.TokenHash == hash);

        if (storedToken is null)
        {
            return;
        }

        storedToken.Revoke();
        await dbContext.SaveChangesAsync();
    }

    public async Task<Result<UserResponse>> GetByIdAsync(Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        return user is null
            ? UserErrors.NotFound
            : UserResponse.FromUser(user);
    }

    private async Task<AuthResponse> IssueTokensAsync(User user)
    {
        var roles = await userManager.GetRolesAsync(user);
        var accessToken = tokenService.CreateAccessToken(user, roles);
        var refreshToken = tokenService.CreateRefreshToken();

        dbContext.RefreshTokens.Add(RefreshToken.Create(user.Id, refreshToken.Hash, refreshToken.ExpiresAtUtc));
        await dbContext.SaveChangesAsync();

        return new AuthResponse(accessToken.Token, accessToken.ExpiresAtUtc, refreshToken.Value);
    }
}