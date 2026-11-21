using Microsoft.EntityFrameworkCore;

namespace PlaytestHub.Shared.Infrastructure.Database;

internal sealed class DbContextMigrator<TContext>(TContext context) : IDatabaseMigrator
    where TContext : DbContext
{
    public Task MigrateAsync(CancellationToken cancellationToken) =>
        context.Database.MigrateAsync(cancellationToken);
}