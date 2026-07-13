using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VisualCron.Application.Outlook;

namespace VisualCron.Infrastructure.Outlook;

public sealed class FailureMailDiscoveryService : IFailureMailDiscoveryService
{
    private readonly IOutlookMailboxService _mailboxService;
    private readonly ILogger<FailureMailDiscoveryService> _logger;
    private readonly string _failureMailPrefix;
    private readonly int _maxEmailsToScan;

    public FailureMailDiscoveryService(IOutlookMailboxService mailboxService, IOptions<OutlookOptions> options, ILogger<FailureMailDiscoveryService> logger)
    {
        _mailboxService = mailboxService;
        _logger = logger;
        _failureMailPrefix = options.Value.FailureMailSubjectPrefix ?? string.Empty;
        _maxEmailsToScan = options.Value.MaxEmailsToScan > 0 ? options.Value.MaxEmailsToScan : 50;
    }

    public async Task<IReadOnlyList<FailureMailDiscoveryItem>> DiscoverAsync(CancellationToken cancellationToken = default)
    {
        var discovered = new List<FailureMailDiscoveryItem>();

        try
        {
            dynamic outlook = await _mailboxService.ConnectAndValidateMailboxAsync(cancellationToken);
            dynamic namespaceObject = outlook.GetNamespace("MAPI");
            dynamic inboxFolder = namespaceObject.GetDefaultFolder(6);

            if (inboxFolder is null)
            {
                throw new InvalidOperationException("Inbox not found.");
            }

            dynamic items = inboxFolder.Items;
            items.Sort("[ReceivedTime]", true);

            int itemCount = items.Count;
            for (int index = 1; index <= itemCount && discovered.Count < _maxEmailsToScan; index++)
            {
                try
                {
                    dynamic item = items.Item(index);
                    string subject = item.Subject ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(_failureMailPrefix) && !subject.StartsWith(_failureMailPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    string? entryId = item.EntryID as string;
                    if (string.IsNullOrWhiteSpace(entryId))
                    {
                        entryId = item.EntryId as string;
                    }

                    string? internetMessageId = TryGetOptionalString(item, "InternetMessageId");
                    if (string.IsNullOrWhiteSpace(internetMessageId))
                    {
                        internetMessageId = TryGetOptionalString(item, "InternetMessageID");
                    }

                    string sender = item.SenderEmailAddress as string ?? item.SenderEmail as string ?? item.SenderName as string ?? string.Empty;
                    DateTimeOffset? receivedTime = null;
                    try
                    {
                        object? receivedTimeValue = item.ReceivedTime;
                        receivedTime = receivedTimeValue switch
                        {
                            null => null,
                            DateTime dateTime => dateTime,
                            DateTimeOffset dateTimeOffset => dateTimeOffset,
                            _ => DateTime.TryParse(receivedTimeValue.ToString(), out DateTime parsed) ? parsed : null
                        };
                    }
                    catch
                    {
                        receivedTime = null;
                    }

                    discovered.Add(new FailureMailDiscoveryItem
                    {
                        IncidentId = !string.IsNullOrWhiteSpace(entryId) ? entryId : internetMessageId,
                        EntryId = entryId,
                        InternetMessageId = internetMessageId,
                        Subject = subject,
                        Sender = sender,
                        ReceivedTime = receivedTime,
                        MailItem = item
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Unable to inspect one Inbox item.");
                }
            }

            FailureMailDiscoveryItem? latestMail = discovered.FirstOrDefault();
            _logger.LogInformation("Latest Mail Subject: {Subject}", latestMail?.Subject ?? string.Empty);
            _logger.LogInformation("Latest Mail ReceivedTime: {ReceivedTime}", latestMail?.ReceivedTime?.ToString("O") ?? string.Empty);
            _logger.LogInformation("Inbox scanned");
            _logger.LogInformation("Failure mails found: {Count}", discovered.Count);
            return discovered;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to discover failure mails from Outlook inbox.");
            return discovered;
        }
    }

    private static string? TryGetOptionalString(dynamic item, string propertyName)
    {
        try
        {
            return item.GetType().GetProperty(propertyName)?.GetValue(item) as string;
        }
        catch
        {
            return null;
        }
    }

}
