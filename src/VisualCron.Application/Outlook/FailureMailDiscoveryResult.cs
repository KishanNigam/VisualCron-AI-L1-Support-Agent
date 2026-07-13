namespace VisualCron.Application.Outlook;

public sealed class FailureMailDiscoveryResult
{
    public int InboxScanned { get; set; }

    public int FailureMailsFound { get; set; }

    public int AlreadyProcessedCount { get; set; }

    public int NewMailCount { get; set; }

    public int SkippedCount { get; set; }

    public IReadOnlyList<FailureMailDiscoveryItem> NewMails { get; set; } = Array.Empty<FailureMailDiscoveryItem>();
}
