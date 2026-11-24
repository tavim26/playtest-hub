using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlaytestHub.Modules.Studio.Domain;

namespace PlaytestHub.Modules.Studio.Infrastructure.Database.Configurations;

internal sealed class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.ToTable("Games");
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Id).ValueGeneratedNever();

        builder.Property(g => g.Title).HasMaxLength(120).IsRequired();
        builder.Property(g => g.Description).HasMaxLength(2000).IsRequired();
        builder.Property(g => g.Genre).HasConversion<string>().HasMaxLength(30);
        builder.Property(g => g.MinimumSpecs).HasMaxLength(1000);

        builder.HasIndex(g => g.OwnerId);

        builder.HasMany(g => g.Members)
            .WithOne()
            .HasForeignKey(m => m.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(g => g.DomainEvents);
    }
}