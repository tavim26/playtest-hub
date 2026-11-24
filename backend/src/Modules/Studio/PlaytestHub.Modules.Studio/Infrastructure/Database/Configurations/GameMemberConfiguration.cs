using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlaytestHub.Modules.Studio.Domain;

namespace PlaytestHub.Modules.Studio.Infrastructure.Database.Configurations;

internal sealed class GameMemberConfiguration : IEntityTypeConfiguration<GameMember>
{
    public void Configure(EntityTypeBuilder<GameMember> builder)
    {
        builder.ToTable("GameMembers");
        builder.HasKey(m => new { m.GameId, m.UserId });

        builder.Property(m => m.Role).HasConversion<string>().HasMaxLength(20);

        builder.HasIndex(m => m.UserId);
    }
}