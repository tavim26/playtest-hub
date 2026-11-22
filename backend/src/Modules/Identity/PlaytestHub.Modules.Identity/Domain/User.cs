using Microsoft.AspNetCore.Identity;

namespace PlaytestHub.Modules.Identity.Domain;

internal sealed class User : IdentityUser<Guid>
{
    public string DisplayName { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;
}