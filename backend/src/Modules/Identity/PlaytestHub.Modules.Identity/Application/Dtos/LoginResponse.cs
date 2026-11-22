namespace PlaytestHub.Modules.Identity.Application.Dtos;

internal sealed record LoginResponse(string AccessToken, DateTime ExpiresAtUtc);