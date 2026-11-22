namespace PlaytestHub.Shared.Infrastructure.Database;

internal interface IDatabaseMigrator
{
    Task MigrateAsync(CancellationToken cancellationToken);
}