using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VisualCron.Application.Outlook;

namespace VisualCron.Infrastructure.Outlook;

public sealed class OutlookMailboxService : IOutlookMailboxService
{
    private readonly OutlookOptions _options;
    private readonly ILogger<OutlookMailboxService> _logger;

    public OutlookMailboxService(IOptions<OutlookOptions> options, ILogger<OutlookMailboxService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<object> ConnectAndValidateMailboxAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Application Start");
        _logger.LogInformation("Connecting to Outlook");

        try
        {
            Type? outlookType = Type.GetTypeFromProgID("Outlook.Application");
            if (outlookType is null)
            {
                throw new InvalidOperationException("Outlook is not installed or not registered on this machine.");
            }

            dynamic outlook = Activator.CreateInstance(outlookType) ?? throw new InvalidOperationException("Unable to create an Outlook application instance.");

            string mailboxName = _options.Mailbox ?? string.Empty;
            _logger.LogInformation("Configured Mailbox: {MailboxName}", string.IsNullOrWhiteSpace(mailboxName) ? "NULL" : mailboxName);
            if (string.IsNullOrWhiteSpace(mailboxName))
            {
                _logger.LogWarning("Mailbox Missing: {MailboxName}", mailboxName);
                throw new InvalidOperationException("Configured Outlook mailbox is missing. Please set Outlook:Mailbox in configuration.");
            }

            dynamic? namespaces = outlook.GetNamespace("MAPI");
            if (namespaces is null)
            {
                throw new InvalidOperationException("Unable to access Outlook namespaces.");
            }

            dynamic? stores = namespaces.Stores;
            if (stores is null)
            {
                throw new InvalidOperationException("Unable to access Outlook stores.");
            }

            bool mailboxFound = false;
            foreach (dynamic store in stores)
            {
                if (string.Equals((string)store.DisplayName, mailboxName, StringComparison.OrdinalIgnoreCase))
                {
                    mailboxFound = true;
                    break;
                }
            }

            if (!mailboxFound)
            {
                throw new InvalidOperationException($"Configured mailbox '{mailboxName}' was not found in the current Outlook profile.");
            }

            _logger.LogInformation("Mailbox Found: {MailboxName}", mailboxName);
            await Task.CompletedTask;
            return outlook;
        }
        catch (Exception ex) when (ex is InvalidOperationException || ex is MissingMethodException || ex is NotSupportedException || ex is ArgumentException)
        {
            _logger.LogError(ex, "Connection Failed: {Message}", ex.Message);
            throw new InvalidOperationException($"Outlook connection validation failed: {ex.Message}", ex);
        }
        finally
        {
            _logger.LogInformation("Application Exit");
        }
    }
}
