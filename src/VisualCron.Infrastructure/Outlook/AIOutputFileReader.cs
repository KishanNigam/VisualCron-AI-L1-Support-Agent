using Microsoft.Extensions.Logging;
using VisualCron.Application.Outlook;

namespace VisualCron.Infrastructure.Outlook;

public sealed class AIOutputFileReader : IAIOutputFileReader
{
    private readonly ILogger<AIOutputFileReader> _logger;

    public AIOutputFileReader(ILogger<AIOutputFileReader> logger)
    {
        _logger = logger;
    }

    public Task<AIOutputReadResult> ReadAsync(string filePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        string normalizedPath = Path.GetFullPath(filePath);
        if (!File.Exists(normalizedPath))
        {
            string message = $"AI output file does not exist: {normalizedPath}";
            _logger.LogError(message);
            throw new InvalidOperationException(message);
        }

        FileInfo fileInfo = new(normalizedPath);
        if (fileInfo.Length <= 0)
        {
            string message = $"AI output file is empty: {normalizedPath}";
            _logger.LogError(message);
            throw new InvalidOperationException(message);
        }

        string content = File.ReadAllText(normalizedPath);
        if (string.IsNullOrWhiteSpace(content))
        {
            string message = $"AI output file is empty: {normalizedPath}";
            _logger.LogError(message);
            throw new InvalidOperationException(message);
        }

        var result = new AIOutputReadResult
        {
            FullContent = content,
            FilePath = normalizedPath,
            FileSize = fileInfo.Length,
            ReadTime = DateTimeOffset.UtcNow
        };

        _logger.LogInformation("AI output file read successfully. Path: {FilePath} Size: {FileSize} Characters: {CharacterCount}", normalizedPath, result.FileSize, content.Length);
        return Task.FromResult(result);
    }
}
