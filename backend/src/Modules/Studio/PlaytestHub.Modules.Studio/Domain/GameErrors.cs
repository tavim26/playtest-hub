using PlaytestHub.Shared.Kernel;

namespace PlaytestHub.Modules.Studio.Domain;

internal static class GameErrors
{
    public static readonly Error NotFound =
        Error.NotFound("Games.NotFound", "The game was not found.");

    public static readonly Error NotOwner =
        Error.Forbidden("Games.NotOwner", "Only the game owner can perform this action.");

    public static readonly Error UserNotFound =
        Error.NotFound("Games.UserNotFound", "No user with this email exists.");

    public static readonly Error AlreadyMember =
        Error.Conflict("Games.AlreadyMember", "This user is already a member of the game.");

    public static readonly Error NotMember =
        Error.NotFound("Games.NotMember", "This user is not a member of the game.");

    public static readonly Error CannotRemoveOwner =
        Error.Validation("Games.CannotRemoveOwner", "The owner cannot be removed from the game.");
}