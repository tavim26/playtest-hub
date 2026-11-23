namespace PlaytestHub.Modules.Identity.Application.Dtos;

internal sealed record RegisterRequest(string Email, string Password, string DisplayName);