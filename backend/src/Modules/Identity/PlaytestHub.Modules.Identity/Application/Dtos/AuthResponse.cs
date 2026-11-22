namespace PlaytestHub.Modules.Identity.Application.Dtos;

internal sealed record AuthResponse(string AccessToken, DateTime AccessTokenExpiresAtUtc, string RefreshToken);