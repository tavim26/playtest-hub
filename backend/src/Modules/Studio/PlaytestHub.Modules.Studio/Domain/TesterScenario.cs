namespace PlaytestHub.Modules.Studio.Domain;

internal sealed record ScenarioDraft(string Title, string Description);

internal sealed class TestScenario
{
    private TestScenario()
    {
    }

    public Guid Id { get; private set; }
    public Guid CampaignId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int Position { get; private set; }

    public static TestScenario Create(Guid campaignId, string title, string description, int position) => new()
    {
        Id = Guid.CreateVersion7(),
        CampaignId = campaignId,
        Title = title,
        Description = description,
        Position = position
    };
}