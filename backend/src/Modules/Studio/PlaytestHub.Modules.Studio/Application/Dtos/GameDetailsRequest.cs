using PlaytestHub.Modules.Studio.Domain;

namespace PlaytestHub.Modules.Studio.Application.Dtos;

internal sealed record GameDetailsRequest(
    string Title,
    string Description,
    SubGenre Genre,
    IReadOnlyList<Platforms> Platforms,
    string? MinimumSpecs);