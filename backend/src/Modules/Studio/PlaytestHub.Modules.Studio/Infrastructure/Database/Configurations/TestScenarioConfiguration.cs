using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlaytestHub.Modules.Studio.Domain;

namespace PlaytestHub.Modules.Studio.Infrastructure.Database.Configurations;

internal sealed class TestScenarioConfiguration : IEntityTypeConfiguration<TestScenario>
{
    public void Configure(EntityTypeBuilder<TestScenario> builder)
    {
        builder.ToTable("TestScenarios");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id).ValueGeneratedNever();

        builder.Property(s => s.Title).HasMaxLength(120).IsRequired();
        builder.Property(s => s.Description).HasMaxLength(1000).IsRequired();
    }
}