using PlaytestHub.Modules.Identity.Domain;

namespace PlaytestHub.Modules.Identity.Application.Dtos;

internal sealed record UserResponse(Guid Id, string Email, string DisplayName)
{
    public static UserResponse FromUser(User user) => new(user.Id, user.Email!, user.DisplayName);
}