namespace VisualCron.Application.Outlook;

public interface IProcessingHistoryRepository
{
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    Task SaveAsync(ProcessedEmailRecord record, CancellationToken cancellationToken = default);
}
