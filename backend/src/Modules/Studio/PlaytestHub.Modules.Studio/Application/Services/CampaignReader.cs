using Microsoft.EntityFrameworkCore;
using PlaytestHub.Modules.Studio.Contracts;
using PlaytestHub.Modules.Studio.Domain;
using PlaytestHub.Modules.Studio.Infrastructure.Database;

namespace PlaytestHub.Modules.Studio.Application.Services;

internal sealed class CampaignReader(StudioDbContext dbContext) : ICampaignReader
{
    public async Task<CampaignInfo?> GetByIdAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return await (
                from c in dbContext.Campaigns.AsNoTracking()
                join g in dbContext.Games.AsNoTracking() on c.GameId equals g.Id
                where c.Id == campaignId
                select new CampaignInfo(
                    c.Id,
                    c.GameId,
                    g.OwnerId,
                    c.Seats,
                    c.MinimumReputation,
                    c.StartsAtUtc,
                    c.EndsAtUtc,
                    c.Status == CampaignStatus.Published && c.StartsAtUtc <= now && c.EndsAtUtc > now))
            .SingleOrDefaultAsync(cancellationToken);
    }
}