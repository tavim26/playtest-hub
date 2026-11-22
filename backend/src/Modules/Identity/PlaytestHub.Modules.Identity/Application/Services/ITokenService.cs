using PlaytestHub.Modules.Identity.Domain;

namespace PlaytestHub.Modules.Identity.Application.Services;

internal sealed record AccessToken(string Token, DateTime ExpiresAtUtc);

internal interface ITokenService
{
    AccessToken CreateAccessToken(User user, IEnumerable<string> roles);
}
