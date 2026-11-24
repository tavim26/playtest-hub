namespace PlaytestHub.Modules.Studio.Domain;

internal sealed class GameMember
{
    private GameMember()
    {
    }

    public Guid GameId { get; private set; }
    public Guid UserId { get; private set; }
    public GameRole Role { get; private set; }
    public DateTime JoinedAtUtc { get; private set; }

    public static GameMember Create(Guid gameId, Guid userId, GameRole role) => new()
    {
        GameId = gameId,
        UserId = userId,
        Role = role,
        JoinedAtUtc = DateTime.UtcNow
    };
}