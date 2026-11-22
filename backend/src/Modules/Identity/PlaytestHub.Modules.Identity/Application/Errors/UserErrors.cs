using PlaytestHub.Shared.Kernel;

namespace PlaytestHub.Modules.Identity.Application.Errors;

internal static class UserErrors
{
    public static readonly Error EmailAlreadyInUse =
        Error.Conflict("Users.EmailAlreadyInUse", "An account with this email already exists.");

    // Same message for unknown email and wrong password, so attackers cannot discover accounts
    public static readonly Error InvalidCredentials =
        Error.Unauthorized("Users.InvalidCredentials", "Invalid email or password.");

    public static readonly Error NotFound =
        Error.NotFound("Users.NotFound", "The user was not found.");

    public static Error CreationFailed(string description) =>
        Error.Validation("Users.CreationFailed", description);
}