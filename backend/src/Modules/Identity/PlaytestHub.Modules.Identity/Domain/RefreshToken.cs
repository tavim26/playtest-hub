using PlaytestHub.Shared.Kernel;

namespace PlaytestHub.Modules.Identity.Domain;

internal sealed class RefreshToken : Entity
{
    private RefreshToken()
    {
        // Required by EF Core
    }

    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }

    public bool IsActive => RevokedAtUtc is null && ExpiresAtUtc > DateTime.UtcNow;

    public static RefreshToken Create(Guid userId, string tokenHash, DateTime expiresAtUtc) => new()
    {
        UserId = userId,
        TokenHash = tokenHash,
        ExpiresAtUtc = expiresAtUtc,
        CreatedAtUtc = DateTime.UtcNow
    };

    public void Revoke() => RevokedAtUtc ??= DateTime.UtcNow;
}