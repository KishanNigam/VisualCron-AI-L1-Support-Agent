namespace VisualCron.Application.Outlook;

public sealed class LogFileReadResult
{
    public string LogFileName { get; set; } = string.Empty;

    public string FullPath { get; set; } = string.Empty;

    public string ExecutionFolder { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public int LineCount { get; set; }

    public long FileSize { get; set; }
}
