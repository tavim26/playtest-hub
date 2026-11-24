using Microsoft.EntityFrameworkCore;
using PlaytestHub.Modules.Studio.Application.Dtos;
using PlaytestHub.Modules.Studio.Domain;
using PlaytestHub.Modules.Studio.Infrastructure.Database;
using PlaytestHub.Shared.Kernel;

namespace PlaytestHub.Modules.Studio.Application.Services;

internal sealed class CampaignCatalog(StudioDbContext dbContext) : ICampaignCatalog
{
    private const int MaxPageSize = 50;

    public async Task<PagedResponse<CampaignListItemResponse>> SearchAsync(CampaignSearch search)
    {
        var page = Math.Max(search.Page, 1);
        var pageSize = Math.Clamp(search.PageSize, 1, MaxPageSize);
        var now = DateTime.UtcNow;

        var query =
            from c in dbContext.Campaigns.AsNoTracking()
            join g in dbContext.Games.AsNoTracking() on c.GameId equals g.Id
            where c.Status == CampaignStatus.Published && c.EndsAtUtc > now
            select new { Campaign = c, Game = g };

        // Filters are added only when the caller provided them
        if (search.Genre is { } genre)
        {
            query = query.Where(x => x.Game.Genre == genre);
        }

        if (search.Platform is { } platform)
        {
            query = query.Where(x => x.Game.Platforms.HasFlag(platform));
        }

        if (!string.IsNullOrWhiteSpace(search.Text))
        {
            var term = search.Text.Trim().ToLower();
            query = query.Where(x => x.Game.Title.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(x => x.Campaign.PublishedAtUtc)
            .ThenBy(x => x.Campaign.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => CampaignListItemResponse.From(x.Campaign, x.Game))
            .ToListAsync();

        return new PagedResponse<CampaignListItemResponse>(items, page, pageSize, totalCount);
    }

    public async Task<Result<PublicCampaignDetailsResponse>> GetPublishedAsync(Guid campaignId)
    {
        var campaign = await dbContext.Campaigns
            .AsNoTracking()
            .Include(c => c.Scenarios)
            .SingleOrDefaultAsync(c => c.Id == campaignId);

        if (campaign is null || campaign.Status != CampaignStatus.Published)
        {
            return CampaignErrors.NotFound;
        }

        var game = await dbContext.Games.AsNoTracking().SingleAsync(g => g.Id == campaign.GameId);

        return PublicCampaignDetailsResponse.From(campaign, game);
    }

    public async Task<IReadOnlyList<CampaignListItemResponse>> GetPendingAsync()
    {
        // Oldest first: the moderation queue is processed in submission order
        return await (
                from c in dbContext.Campaigns.AsNoTracking()
                join g in dbContext.Games.AsNoTracking() on c.GameId equals g.Id
                where c.Status == CampaignStatus.PendingReview
                orderby c.CreatedAtUtc
                select new { Campaign = c, Game = g })
            .Take(100)
            .Select(x => CampaignListItemResponse.From(x.Campaign, x.Game))
            .ToListAsync();
    }
}