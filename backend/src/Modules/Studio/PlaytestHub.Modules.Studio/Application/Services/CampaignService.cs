using Microsoft.EntityFrameworkCore;
using PlaytestHub.Modules.Studio.Application.Dtos;
using PlaytestHub.Modules.Studio.Domain;
using PlaytestHub.Modules.Studio.Infrastructure.Database;
using PlaytestHub.Shared.Kernel;

namespace PlaytestHub.Modules.Studio.Application.Services;

internal sealed class CampaignService(StudioDbContext dbContext) : ICampaignService
{
    public async Task<Result<CampaignResponse>> CreateAsync(Guid gameId, CampaignDetailsRequest request, Guid userId)
    {
        var game = await dbContext.Games.AsNoTracking().SingleOrDefaultAsync(g => g.Id == gameId);

        if (game is null)
        {
            return GameErrors.NotFound;
        }

        if (!game.IsOwner(userId))
        {
            return GameErrors.NotOwner;
        }

        var campaign = Campaign.Create(
            gameId,
            request.BuildVersion,
            request.Description,
            request.Seats,
            request.MinimumReputation,
            request.StartsAt.UtcDateTime,
            request.EndsAt.UtcDateTime,
            ToDrafts(request));

        dbContext.Campaigns.Add(campaign);
        await dbContext.SaveChangesAsync();

        return CampaignResponse.FromCampaign(campaign);
    }

    public async Task<Result<IReadOnlyList<CampaignSummaryResponse>>> GetByGameAsync(Guid gameId, Guid userId)
    {
        var isMember = await dbContext.Games
            .AnyAsync(g => g.Id == gameId && g.Members.Any(m => m.UserId == userId));

        if (!isMember)
        {
            return GameErrors.NotFound;
        }

        IReadOnlyList<CampaignSummaryResponse> campaigns = await dbContext.Campaigns
            .AsNoTracking()
            .Where(c => c.GameId == gameId)
            .OrderByDescending(c => c.CreatedAtUtc)
            .Select(c => new CampaignSummaryResponse(
                c.Id, c.GameId, c.BuildVersion, c.Seats, c.StartsAtUtc, c.EndsAtUtc, c.Status))
            .ToListAsync();

        return Result.Success(campaigns);
    }

    public async Task<Result<CampaignResponse>> GetByIdAsync(Guid campaignId, Guid userId, bool isModerator)
    {
        var campaign = await dbContext.Campaigns
            .AsNoTracking()
            .Include(c => c.Scenarios)
            .SingleOrDefaultAsync(c => c.Id == campaignId);

        if (campaign is null)
        {
            return CampaignErrors.NotFound;
        }

        if (!isModerator)
        {
            var isMember = await dbContext.Games
                .AnyAsync(g => g.Id == campaign.GameId && g.Members.Any(m => m.UserId == userId));

            // Same answer as "does not exist", so outsiders cannot probe for campaign ids
            if (!isMember)
            {
                return CampaignErrors.NotFound;
            }
        }

        return CampaignResponse.FromCampaign(campaign);
    }

    public async Task<Result<CampaignResponse>> UpdateAsync(Guid campaignId, CampaignDetailsRequest request, Guid userId)
    {
        var loaded = await LoadOwnedAsync(campaignId, userId);

        if (loaded.IsFailure)
        {
            return loaded.Error;
        }

        return await ApplyAsync(loaded.Value, campaign => campaign.UpdateDetails(
            request.BuildVersion,
            request.Description,
            request.Seats,
            request.MinimumReputation,
            request.StartsAt.UtcDateTime,
            request.EndsAt.UtcDateTime,
            ToDrafts(request)));
    }

    public async Task<Result<CampaignResponse>> SubmitAsync(Guid campaignId, Guid userId)
    {
        var loaded = await LoadOwnedAsync(campaignId, userId);

        if (loaded.IsFailure)
        {
            return loaded.Error;
        }

        return await ApplyAsync(loaded.Value, campaign => campaign.Submit());
    }

    public async Task<Result<CampaignResponse>> CloseAsync(Guid campaignId, Guid userId)
    {
        var loaded = await LoadOwnedAsync(campaignId, userId);

        if (loaded.IsFailure)
        {
            return loaded.Error;
        }

        return await ApplyAsync(loaded.Value, campaign => campaign.Close());
    }

    public async Task<Result<CampaignResponse>> ApproveAsync(Guid campaignId)
    {
        var campaign = await LoadAsync(campaignId);

        if (campaign is null)
        {
            return CampaignErrors.NotFound;
        }

        return await ApplyAsync(campaign, c => c.Approve());
    }

    public async Task<Result<CampaignResponse>> RejectAsync(Guid campaignId, RejectCampaignRequest request)
    {
        var campaign = await LoadAsync(campaignId);

        if (campaign is null)
        {
            return CampaignErrors.NotFound;
        }

        return await ApplyAsync(campaign, c => c.Reject(request.Reason));
    }

    private Task<Campaign?> LoadAsync(Guid campaignId) =>
        dbContext.Campaigns
            .Include(c => c.Scenarios)
            .SingleOrDefaultAsync(c => c.Id == campaignId);

    private async Task<Result<Campaign>> LoadOwnedAsync(Guid campaignId, Guid userId)
    {
        var campaign = await LoadAsync(campaignId);

        if (campaign is null)
        {
            return CampaignErrors.NotFound;
        }

        var ownerId = await dbContext.Games
            .Where(g => g.Id == campaign.GameId)
            .Select(g => g.OwnerId)
            .SingleAsync();

        if (ownerId != userId)
        {
            return GameErrors.NotOwner;
        }

        return campaign;
    }

    // Runs a domain operation, saves on success and maps the campaign to its response
    private async Task<Result<CampaignResponse>> ApplyAsync(Campaign campaign, Func<Campaign, Result> operation)
    {
        var result = operation(campaign);

        if (result.IsFailure)
        {
            return result.Error;
        }

        await dbContext.SaveChangesAsync();

        return CampaignResponse.FromCampaign(campaign);
    }

    private static List<ScenarioDraft> ToDrafts(CampaignDetailsRequest request) =>
        request.Scenarios.Select(s => new ScenarioDraft(s.Title, s.Description)).ToList();
}