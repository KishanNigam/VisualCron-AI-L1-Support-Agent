using Microsoft.Extensions.Logging;
using VisualCron.Application.Outlook;

namespace VisualCron.Infrastructure.Outlook;

public sealed class FailureMailAttachmentDownloader : IFailureMailAttachmentDownloader
{
    private readonly ILogger<FailureMailAttachmentDownloader> _logger;

    public FailureMailAttachmentDownloader(ILogger<FailureMailAttachmentDownloader> logger)
    {
        _logger = logger;
    }

    public Task<IReadOnlyList<DownloadedAttachment>> DownloadAsync(object mail, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(mail);

        dynamic mailItem = mail;
        dynamic attachments = mailItem.Attachments;
        int count = attachments.Count;

        if (count <= 0)
        {
            _logger.LogInformation("Attachment Count: 0");
            return Task.FromResult<IReadOnlyList<DownloadedAttachment>>(Array.Empty<DownloadedAttachment>());
        }

        string executionId = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        string runDirectory = Path.Combine(AppContext.BaseDirectory, "runtime", "temp", executionId);
        Directory.CreateDirectory(runDirectory);

        var downloaded = new List<DownloadedAttachment>();
        for (int index = 1; index <= count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            dynamic attachment = attachments.Item(index);
            string? fileName = attachment.FileName as string;
            if (string.IsNullOrWhiteSpace(fileName))
            {
                continue;
            }

            if (!fileName.EndsWith(".log", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("Attachment Name: {AttachmentName}", fileName);
                _logger.LogInformation("Skipping non-log attachment.");
                continue;
            }

            string destinationPath = Path.Combine(runDirectory, Path.GetFileName(fileName));
            attachment.SaveAsFile(destinationPath);

            downloaded.Add(new DownloadedAttachment
            {
                FileName = Path.GetFileName(fileName),
                FullPath = destinationPath,
                FileSize = new FileInfo(destinationPath).Length,
                Extension = Path.GetExtension(fileName)
            });

            _logger.LogInformation("Attachment Count: {Count}", downloaded.Count);
            _logger.LogInformation("Attachment Name: {AttachmentName}", Path.GetFileName(fileName));
            _logger.LogInformation("Destination Path: {DestinationPath}", destinationPath);
        }

        _logger.LogInformation("Completed. Execution Folder: {ExecutionFolder}", runDirectory);
        return Task.FromResult<IReadOnlyList<DownloadedAttachment>>(downloaded);
    }
}
