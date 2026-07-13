namespace VisualCron.Application.Outlook;

public sealed class DownloadedAttachment
{
    public string FileName { get; set; } = string.Empty;

    public string FullPath { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public string Extension { get; set; } = string.Empty;
}
