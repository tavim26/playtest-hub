using Microsoft.EntityFrameworkCore;
using PlaytestHub.Modules.Studio.Domain;


namespace PlaytestHub.Modules.Studio.Infrastructure.Database;

internal sealed class StudioDbContext(DbContextOptions<StudioDbContext> options) : DbContext(options)
{
    public const string Schema = "studio";

    public DbSet<Game> Games => Set<Game>();
    public DbSet<Campaign> Campaigns => Set<Campaign>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StudioDbContext).Assembly);
    }
}