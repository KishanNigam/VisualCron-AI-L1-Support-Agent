namespace VisualCron.Application.Outlook;

public sealed class AIOutputReadResult
{
    public string FullContent { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public DateTimeOffset ReadTime { get; set; }
}
