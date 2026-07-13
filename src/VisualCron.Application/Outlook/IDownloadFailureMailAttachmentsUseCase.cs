namespace VisualCron.Application.Outlook;

public interface IDownloadFailureMailAttachmentsUseCase
{
    Task<IReadOnlyList<DownloadedAttachment>> ExecuteAsync(object mail, CancellationToken cancellationToken = default);
}
