using Microsoft.Extensions.Logging;

namespace VisualCron.Application.Outlook;

public sealed class DownloadFailureMailAttachmentsUseCase : IDownloadFailureMailAttachmentsUseCase
{
    private readonly IFailureMailAttachmentDownloader _downloader;
    private readonly ILogger<DownloadFailureMailAttachmentsUseCase> _logger;

    public DownloadFailureMailAttachmentsUseCase(IFailureMailAttachmentDownloader downloader, ILogger<DownloadFailureMailAttachmentsUseCase> logger)
    {
        _downloader = downloader;
        _logger = logger;
    }

    public async Task<IReadOnlyList<DownloadedAttachment>> ExecuteAsync(object mail, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<DownloadedAttachment> attachments = await _downloader.DownloadAsync(mail, cancellationToken);
        _logger.LogInformation("Attachment Count: {Count}", attachments.Count);
        foreach (DownloadedAttachment attachment in attachments)
        {
            _logger.LogInformation("Downloaded File Name: {FileName}", attachment.FileName);
            _logger.LogInformation("Download Path: {Path}", attachment.FullPath);
        }

        return attachments;
    }
}
