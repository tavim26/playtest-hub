using PlaytestHub.Modules.Studio.Contracts;
using PlaytestHub.Shared.Kernel;

namespace PlaytestHub.Modules.Studio.Domain;

internal sealed class Campaign : Entity
{
    private readonly List<TestScenario> _scenarios = [];

    private Campaign()
    {
        // Required by EF Core
    }

    public Guid GameId { get; private set; }
    public string BuildVersion { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int Seats { get; private set; }
    public int MinimumReputation { get; private set; }
    public DateTime StartsAtUtc { get; private set; }
    public DateTime EndsAtUtc { get; private set; }
    public CampaignStatus Status { get; private set; }
    public string? RejectionReason { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? PublishedAtUtc { get; private set; }

    public IReadOnlyCollection<TestScenario> Scenarios => _scenarios;

    public static Campaign Create(
        Guid gameId,
        string buildVersion,
        string description,
        int seats,
        int minimumReputation,
        DateTime startsAtUtc,
        DateTime endsAtUtc,
        IEnumerable<ScenarioDraft> scenarios)
    {
        var campaign = new Campaign
        {
            GameId = gameId,
            Status = CampaignStatus.Draft,
            CreatedAtUtc = DateTime.UtcNow
        };

        campaign.ApplyDetails(buildVersion, description, seats, minimumReputation, startsAtUtc, endsAtUtc, scenarios);

        return campaign;
    }

    public Result UpdateDetails(
        string buildVersion,
        string description,
        int seats,
        int minimumReputation,
        DateTime startsAtUtc,
        DateTime endsAtUtc,
        IEnumerable<ScenarioDraft> scenarios)
    {
        if (Status is not (CampaignStatus.Draft or CampaignStatus.Rejected))
        {
            return Result.Failure(CampaignErrors.NotEditable);
        }

        ApplyDetails(buildVersion, description, seats, minimumReputation, startsAtUtc, endsAtUtc, scenarios);

        return Result.Success();
    }

    public Result Submit()
    {
        if (Status is not (CampaignStatus.Draft or CampaignStatus.Rejected))
        {
            return Result.Failure(CampaignErrors.InvalidTransition(Status, "submitted"));
        }

        if (_scenarios.Count == 0)
        {
            return Result.Failure(CampaignErrors.NoScenarios);
        }

        if (EndsAtUtc <= DateTime.UtcNow)
        {
            return Result.Failure(CampaignErrors.AlreadyEnded);
        }

        Status = CampaignStatus.PendingReview;
        RejectionReason = null;

        return Result.Success();
    }

    public Result Approve()
    {
        if (Status != CampaignStatus.PendingReview)
        {
            return Result.Failure(CampaignErrors.InvalidTransition(Status, "approved"));
        }

        if (EndsAtUtc <= DateTime.UtcNow)
        {
            return Result.Failure(CampaignErrors.AlreadyEnded);
        }

        Status = CampaignStatus.Published;
        PublishedAtUtc = DateTime.UtcNow;
        Raise(new CampaignPublished(Id, GameId, PublishedAtUtc.Value));

        return Result.Success();
    }

    public Result Reject(string reason)
    {
        if (Status != CampaignStatus.PendingReview)
        {
            return Result.Failure(CampaignErrors.InvalidTransition(Status, "rejected"));
        }

        Status = CampaignStatus.Rejected;
        RejectionReason = reason;

        return Result.Success();
    }

    public Result Close()
    {
        if (Status != CampaignStatus.Published)
        {
            return Result.Failure(CampaignErrors.InvalidTransition(Status, "closed"));
        }

        Status = CampaignStatus.Closed;

        return Result.Success();
    }

    private void ApplyDetails(
        string buildVersion,
        string description,
        int seats,
        int minimumReputation,
        DateTime startsAtUtc,
        DateTime endsAtUtc,
        IEnumerable<ScenarioDraft> scenarios)
    {
        BuildVersion = buildVersion;
        Description = description;
        Seats = seats;
        MinimumReputation = minimumReputation;
        StartsAtUtc = startsAtUtc;
        EndsAtUtc = endsAtUtc;

        _scenarios.Clear();

        var position = 1;
        foreach (var draft in scenarios)
        {
            _scenarios.Add(TestScenario.Create(Id, draft.Title, draft.Description, position++));
        }
    }
}