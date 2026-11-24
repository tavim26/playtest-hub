using PlaytestHub.Modules.Identity.Contracts;
using PlaytestHub.Modules.Studio.Domain;

namespace PlaytestHub.Modules.Studio.Application.Dtos;

internal sealed record GameMemberResponse(Guid UserId, string DisplayName, GameRole Role);

internal sealed record GameResponse(
    Guid Id,
    string Title,
    string Description,
    SubGenre Genre,
    IReadOnlyList<Platforms> Platforms,
    string? MinimumSpecs,
    Guid OwnerId,
    DateTime CreatedAtUtc,
    IReadOnlyList<GameMemberResponse> Members)
{
    public static GameResponse FromGame(Game game, IReadOnlyDictionary<Guid, UserSummary> users) => new(
        game.Id,
        game.Title,
        game.Description,
        game.Genre,
        PlatformMapping.ToList(game.Platforms),
        game.MinimumSpecs,
        game.OwnerId,
        game.CreatedAtUtc,
        game.Members
            .OrderBy(m => m.Role)
            .ThenBy(m => m.JoinedAtUtc)
            .Select(m => new GameMemberResponse(
                m.UserId,
                users.GetValueOrDefault(m.UserId)?.DisplayName ?? "Unknown user",
                m.Role))
            .ToList());
}