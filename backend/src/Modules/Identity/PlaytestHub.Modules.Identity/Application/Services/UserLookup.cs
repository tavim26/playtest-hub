using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlaytestHub.Modules.Identity.Contracts;
using PlaytestHub.Modules.Identity.Domain;

namespace PlaytestHub.Modules.Identity.Application.Services;

internal sealed class UserLookup(UserManager<User> userManager) : IUserLookup
{
    public async Task<UserSummary?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);

        return user is null ? null : new UserSummary(user.Id, user.Email!, user.DisplayName);
    }

    public async Task<IReadOnlyDictionary<Guid, UserSummary>> GetByIdsAsync(
        IEnumerable<Guid> userIds,
        CancellationToken cancellationToken = default)
    {
        var ids = userIds.Distinct().ToList();

        return await userManager.Users
            .Where(u => ids.Contains(u.Id))
            .Select(u => new UserSummary(u.Id, u.Email!, u.DisplayName))
            .ToDictionaryAsync(u => u.Id, cancellationToken);
    }
}