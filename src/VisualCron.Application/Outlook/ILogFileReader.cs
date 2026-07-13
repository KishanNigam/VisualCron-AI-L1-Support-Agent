namespace VisualCron.Application.Outlook;

public interface ILogFileReader
{
    Task<IReadOnlyList<LogFileReadResult>> ReadAsync(IEnumerable<string> filePaths, CancellationToken cancellationToken = default);
}
