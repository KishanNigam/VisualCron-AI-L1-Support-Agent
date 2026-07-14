using Microsoft.Extensions.Logging;

namespace VisualCron.Application.Outlook;

public sealed class ReadAIOutputFileUseCase
{
    private readonly IAIOutputFileReader _reader;
    private readonly ILogger<ReadAIOutputFileUseCase> _logger;

    public ReadAIOutputFileUseCase(IAIOutputFileReader reader, ILogger<ReadAIOutputFileUseCase> logger)
    {
        _reader = reader;
        _logger = logger;
    }

    public async Task<AIOutputReadResult> ExecuteAsync(string filePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        AIOutputReadResult result = await _reader.ReadAsync(filePath, cancellationToken);
        _logger.LogInformation("AI output file read. Path: {FilePath} Size: {FileSize} Characters: {CharacterCount}", result.FilePath, result.FileSize, result.FullContent.Length);
        return result;
    }
}
