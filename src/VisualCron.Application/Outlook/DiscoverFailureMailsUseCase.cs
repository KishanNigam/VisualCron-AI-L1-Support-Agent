using Microsoft.Extensions.Logging;

namespace VisualCron.Application.Outlook;

public sealed class DiscoverFailureMailsUseCase
{
    private readonly IFailureMailDiscoveryService _discoveryService;
    private readonly IProcessingHistoryRepository _repository;
    private readonly ILogger<DiscoverFailureMailsUseCase> _logger;

    public DiscoverFailureMailsUseCase(IFailureMailDiscoveryService discoveryService, IProcessingHistoryRepository repository, ILogger<DiscoverFailureMailsUseCase> logger)
    {
        _discoveryService = discoveryService;
        _repository = repository;
        _logger = logger;
    }

    public async Task<FailureMailDiscoveryResult> ExecuteAsync(string executionId, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<FailureMailDiscoveryItem> mails = await _discoveryService.DiscoverAsync(cancellationToken);
        var result = new FailureMailDiscoveryResult
        {
            InboxScanned = mails.Count,
            FailureMailsFound = mails.Count,
            NewMails = new List<FailureMailDiscoveryItem>()
        };

        foreach (FailureMailDiscoveryItem mail in mails)
        {
            string incidentId = GetIncidentId(mail);
            bool exists = await _repository.ExistsAsync(incidentId, cancellationToken);
            if (exists)
            {
                result.AlreadyProcessedCount++;
                result.SkippedCount++;
                continue;
            }

            result.NewMailCount++;
            ((List<FailureMailDiscoveryItem>)result.NewMails).Add(mail);

            var record = new ProcessedEmailRecord
            {
                IncidentId = incidentId,
                EntryId = mail.EntryId ?? string.Empty,
                InternetMessageId = mail.InternetMessageId ?? string.Empty,
                Subject = mail.Subject,
                Sender = mail.Sender,
                ReceivedTime = mail.ReceivedTime,
                ProcessedTime = DateTimeOffset.UtcNow,
                ExecutionId = executionId,
                Status = "Processed"
            };

            await _repository.SaveAsync(record, cancellationToken);
        }

        _logger.LogInformation("Inbox scanned: {Count}", result.InboxScanned);
        _logger.LogInformation("Failure mails found: {Count}", result.FailureMailsFound);
        _logger.LogInformation("Already processed count: {Count}", result.AlreadyProcessedCount);
        _logger.LogInformation("New mails count: {Count}", result.NewMailCount);
        _logger.LogInformation("Skipped count: {Count}", result.SkippedCount);

        return result;
    }

    private static string GetIncidentId(FailureMailDiscoveryItem mail)
    {
        if (!string.IsNullOrWhiteSpace(mail.EntryId))
        {
            return mail.EntryId;
        }

        if (!string.IsNullOrWhiteSpace(mail.InternetMessageId))
        {
            return mail.InternetMessageId;
        }

        return $"{mail.Subject}|{mail.Sender}|{mail.ReceivedTime?.ToString("O") ?? string.Empty}";
    }
}
