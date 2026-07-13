namespace VisualCron.Application.Outlook;

public sealed class FailureMailDiscoveryItem
{
    public string? IncidentId { get; set; }

    public string? EntryId { get; set; }

    public string? InternetMessageId { get; set; }

    public string Subject { get; set; } = string.Empty;

    public string Sender { get; set; } = string.Empty;

    public DateTimeOffset? ReceivedTime { get; set; }

    public object? MailItem { get; set; }
}
