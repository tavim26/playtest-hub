using PlaytestHub.Shared.Kernel;

namespace PlaytestHub.Modules.Studio.Domain;

internal sealed class Game : Entity
{
    private readonly List<GameMember> _members = [];

    private Game()
    {
    }

    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public SubGenre Genre { get; private set; }
    public Platforms Platforms { get; private set; }
    public string? MinimumSpecs { get; private set; }
    public Guid OwnerId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public IReadOnlyCollection<GameMember> Members => _members;

    public static Game Create(
        string title,
        string description,
        SubGenre genre,
        Platforms platforms,
        string? minimumSpecs,
        Guid ownerId)
    {
        var game = new Game
        {
            OwnerId = ownerId,
            CreatedAtUtc = DateTime.UtcNow
        };

        game.UpdateDetails(title, description, genre, platforms, minimumSpecs);
        game._members.Add(GameMember.Create(game.Id, ownerId, GameRole.Owner));

        return game;
    }

    public void UpdateDetails(string title, string description, SubGenre genre, Platforms platforms, string? minimumSpecs)
    {
        Title = title;
        Description = description;
        Genre = genre;
        Platforms = platforms;
        MinimumSpecs = minimumSpecs;
    }

    public bool IsOwner(Guid userId) => OwnerId == userId;

    public bool IsMember(Guid userId) => _members.Any(m => m.UserId == userId);

    public Result AddCollaborator(Guid userId)
    {
        if (IsMember(userId))
        {
            return Result.Failure(GameErrors.AlreadyMember);
        }

        _members.Add(GameMember.Create(Id, userId, GameRole.Collaborator));

        return Result.Success();
    }

    public Result RemoveCollaborator(Guid userId)
    {
        if (IsOwner(userId))
        {
            return Result.Failure(GameErrors.CannotRemoveOwner);
        }

        var member = _members.SingleOrDefault(m => m.UserId == userId);

        if (member is null)
        {
            return Result.Failure(GameErrors.NotMember);
        }

        _members.Remove(member);

        return Result.Success();
    }
}