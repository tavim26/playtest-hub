using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PlaytestHub.Shared.Infrastructure.Database;

public static class DatabaseExtensions
{
    // Each module gets its own DbContext, schema and migrations history table
    public static IServiceCollection AddModuleDbContext<TContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        string schema)
        where TContext : DbContext
    {
        var connectionString = configuration.GetConnectionString("Database")
                               ?? throw new InvalidOperationException("Connection string 'Database' is not configured.");

        services.AddDbContext<TContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable(HistoryRepository.DefaultTableName, schema)));

        services.AddScoped<IDatabaseMigrator, DbContextMigrator<TContext>>();

        return services;
    }

    public static async Task ApplyMigrationsAsync(this IHost app)
    {
        using var scope = app.Services.CreateScope();

        foreach (var migrator in scope.ServiceProvider.GetServices<IDatabaseMigrator>())
        {
            await migrator.MigrateAsync(CancellationToken.None);
        }
    }
}