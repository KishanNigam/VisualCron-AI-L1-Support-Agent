namespace VisualCron.Application.Outlook;

public interface IOutlookMailboxService
{
    Task<object> ConnectAndValidateMailboxAsync(CancellationToken cancellationToken = default);
}
