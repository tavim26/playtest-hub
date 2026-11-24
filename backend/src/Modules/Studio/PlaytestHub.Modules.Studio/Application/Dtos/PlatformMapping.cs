using PlaytestHub.Modules.Studio.Domain;

namespace PlaytestHub.Modules.Studio.Application.Dtos;

// The API uses a list ["Windows", "Linux"], the domain stores combined flags
internal static class PlatformMapping
{
    public static Platforms ToFlags(IEnumerable<Platforms> platforms) =>
        platforms.Aggregate(Platforms.None, (all, platform) => all | platform);

    public static IReadOnlyList<Platforms> ToList(Platforms flags) =>
        Enum.GetValues<Platforms>()
            .Where(p => p != Platforms.None && flags.HasFlag(p))
            .ToList();
}