namespace VisualCron.Application.Outlook;

public sealed class ProcessedEmailRecord
{
    public string IncidentId { get; set; } = string.Empty;

    public string EntryId { get; set; } = string.Empty;

    public string InternetMessageId { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public string Sender { get; set; } = string.Empty;

    public DateTimeOffset? ReceivedTime { get; set; }

    public DateTimeOffset ProcessedTime { get; set; }

    public string ExecutionId { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
}
