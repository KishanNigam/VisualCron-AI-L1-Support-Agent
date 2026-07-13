using Microsoft.Extensions.Logging;
using VisualCron.Application.Outlook;

namespace VisualCron.Infrastructure.Outlook;

public sealed class UnreadEmailReader : IUnreadEmailReader
{
    private readonly IOutlookMailboxService _mailboxService;
    private readonly ILogger<UnreadEmailReader> _logger;

    public UnreadEmailReader(IOutlookMailboxService mailboxService, ILogger<UnreadEmailReader> logger)
    {
        _mailboxService = mailboxService;
        _logger = logger;
    }

    public async Task<IReadOnlyList<UnreadEmailDto>> ReadUnreadEmailsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            object mailbox = await _mailboxService.ConnectAndValidateMailboxAsync(cancellationToken);
            dynamic outlookApplication = mailbox;
            dynamic namespaceObject = outlookApplication.GetNamespace("MAPI");
            dynamic inboxFolder = namespaceObject.GetDefaultFolder(6);

            if (inboxFolder is null)
            {
                throw new InvalidOperationException("Inbox not found.");
            }

            _logger.LogInformation("Inbox Opened");

            dynamic items = inboxFolder.Items;
            items.Sort("[ReceivedTime]", true);

            List<UnreadEmailDto> unreadEmails = new();
            foreach (dynamic item in items)
            {
                if (item.Unread)
                {
                    unreadEmails.Add(new UnreadEmailDto
                    {
                        Subject = item.Subject ?? string.Empty,
                        Sender = item.SenderName ?? string.Empty,
                        ReceivedTime = item.ReceivedTime is DateTime receivedTime ? new DateTimeOffset(receivedTime) : null
                    });
                }
            }

            _logger.LogInformation("Unread Mail Count: {Count}", unreadEmails.Count);
            foreach (UnreadEmailDto mail in unreadEmails)
            {
                _logger.LogInformation("Subject: {Subject}", mail.Subject);
                _logger.LogInformation("Sender: {Sender}", mail.Sender);
                _logger.LogInformation("Received Time: {ReceivedTime}", mail.ReceivedTime?.ToString("u") ?? "N/A");
            }

            return unreadEmails;
        }
        catch (Exception ex) when (ex is InvalidOperationException || ex is MissingMethodException || ex is NotSupportedException || ex is ArgumentException)
        {
            _logger.LogError(ex, "Failed to read unread Outlook emails. {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to read unread Outlook emails: {ex.Message}", ex);
        }
    }
}
