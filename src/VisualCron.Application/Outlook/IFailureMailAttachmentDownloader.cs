namespace VisualCron.Application.Outlook;

public interface IFailureMailAttachmentDownloader
{
    Task<IReadOnlyList<DownloadedAttachment>> DownloadAsync(object mail, CancellationToken cancellationToken = default);
}
