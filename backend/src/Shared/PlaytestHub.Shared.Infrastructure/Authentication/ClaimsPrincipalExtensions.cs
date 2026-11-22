using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace PlaytestHub.Shared.Infrastructure.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        return Guid.TryParse(value, out var userId)
            ? userId
            : throw new InvalidOperationException("The user id claim is missing or invalid.");
    }
}