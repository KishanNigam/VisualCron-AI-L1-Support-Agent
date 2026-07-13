namespace VisualCron.Application.Outlook;

public sealed class UnreadEmailDto
{
    public string Subject { get; set; } = string.Empty;

    public string Sender { get; set; } = string.Empty;

    public DateTimeOffset? ReceivedTime { get; set; }
}
