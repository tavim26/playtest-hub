namespace PlaytestHub.Modules.Identity.Contracts;

public sealed record UserSummary(Guid Id, string Email, string DisplayName);

public interface IUserLookup
{
    Task<UserSummary?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<Guid, UserSummary>> GetByIdsAsync(
        IEnumerable<Guid> userIds,
        CancellationToken cancellationToken = default);
}