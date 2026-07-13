namespace VisualCron.Application.Outlook;

public sealed class OutlookOptions
{
    public const string SectionName = "Outlook";

    public string Mailbox { get; set; } = string.Empty;

    public int MaxEmailsToScan { get; set; } = 50;

    public string? FailureMailSubjectPrefix { get; set; }
}
