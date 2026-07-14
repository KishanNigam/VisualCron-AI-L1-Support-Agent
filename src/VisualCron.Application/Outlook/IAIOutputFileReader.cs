namespace VisualCron.Application.Outlook;

public interface IAIOutputFileReader
{
    Task<AIOutputReadResult> ReadAsync(string filePath, CancellationToken cancellationToken = default);
}
