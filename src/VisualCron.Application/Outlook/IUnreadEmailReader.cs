namespace VisualCron.Application.Outlook;

public interface IUnreadEmailReader
{
    Task<IReadOnlyList<UnreadEmailDto>> ReadUnreadEmailsAsync(CancellationToken cancellationToken = default);
}
