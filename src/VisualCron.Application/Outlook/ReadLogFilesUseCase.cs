using Microsoft.Extensions.Logging;

namespace VisualCron.Application.Outlook;

public sealed class ReadLogFilesUseCase
{
    private readonly ILogFileReader _reader;
    private readonly ILogger<ReadLogFilesUseCase> _logger;

    public ReadLogFilesUseCase(ILogFileReader reader, ILogger<ReadLogFilesUseCase> logger)
    {
        _reader = reader;
        _logger = logger;
    }

    public async Task<IReadOnlyList<LogFileReadResult>> ExecuteAsync(IEnumerable<string> filePaths, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<LogFileReadResult> results = await _reader.ReadAsync(filePaths, cancellationToken);
        _logger.LogInformation("Read {Count} log file(s).", results.Count);
        return results;
    }
}
