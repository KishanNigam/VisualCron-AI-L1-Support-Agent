namespace VisualCron.Application.Outlook;

public sealed class FailureMailMetadata
{
    public string OriginalSubject { get; set; } = string.Empty;

    public string Prefix { get; set; } = string.Empty;

    public string JobName { get; set; } = string.Empty;

    public string Environment { get; set; } = string.Empty;

    public string Server { get; set; } = string.Empty;

    public bool IsValid { get; set; }

    public string ValidationMessage { get; set; } = string.Empty;
}
