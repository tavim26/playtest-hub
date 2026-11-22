using PlaytestHub.Modules.Identity.Domain;

namespace PlaytestHub.Modules.Identity.Application.Services;

internal sealed record AccessToken(string Token, DateTime ExpiresAtUtc);

internal sealed record GeneratedRefreshToken(string Value, string Hash, DateTime ExpiresAtUtc);

internal interface ITokenService
{
    AccessToken CreateAccessToken(User user, IEnumerable<string> roles);
    GeneratedRefreshToken CreateRefreshToken();
    string HashRefreshToken(string refreshToken);
}