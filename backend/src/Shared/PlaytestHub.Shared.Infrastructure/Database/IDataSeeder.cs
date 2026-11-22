namespace PlaytestHub.Shared.Infrastructure.Database;

public interface IDataSeeder
{
    Task SeedAsync(CancellationToken cancellationToken);
}