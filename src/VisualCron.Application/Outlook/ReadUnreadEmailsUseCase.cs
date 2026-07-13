namespace VisualCron.Application.Outlook;

public sealed class ReadUnreadEmailsUseCase
{
    private readonly IUnreadEmailReader _reader;

    public ReadUnreadEmailsUseCase(IUnreadEmailReader reader)
    {
        _reader = reader;
    }

    public Task<IReadOnlyList<UnreadEmailDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return _reader.ReadUnreadEmailsAsync(cancellationToken);
    }
}
