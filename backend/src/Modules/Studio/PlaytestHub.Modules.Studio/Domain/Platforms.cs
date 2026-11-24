namespace PlaytestHub.Modules.Studio.Domain;

// Flags enum: one value can hold several platforms, e.g. Windows | Linux
[Flags]
internal enum Platforms
{
    None = 0,
    Windows = 1,
    MacOs = 2,
    Linux = 4,
    SteamDeck = 8,
    Web = 16
}