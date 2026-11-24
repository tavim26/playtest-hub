using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlaytestHub.Modules.Studio.Domain;

namespace PlaytestHub.Modules.Studio.Infrastructure.Database.Configurations;

internal sealed class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
{
    public void Configure(EntityTypeBuilder<Campaign> builder)
    {
        builder.ToTable("Campaigns");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.BuildVersion).HasMaxLength(40).IsRequired();
        builder.Property(c => c.Description).HasMaxLength(2000).IsRequired();
        builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.RejectionReason).HasMaxLength(1000);

        builder.HasIndex(c => c.GameId);
        builder.HasIndex(c => new { c.Status, c.PublishedAtUtc });

        // Foreign key to Games without a navigation property on Campaign
        builder.HasOne<Game>()
            .WithMany()
            .HasForeignKey(c => c.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Scenarios)
            .WithOne()
            .HasForeignKey(s => s.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(c => c.DomainEvents);
    }
}