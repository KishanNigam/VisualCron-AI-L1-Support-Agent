using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VisualCron.Application.Configuration;
using VisualCron.Application.Outlook;

namespace VisualCron.Infrastructure.Outlook;

public sealed class JsonProcessingHistoryRepository : IProcessingHistoryRepository
{
    private readonly string _historyDirectory;
    private readonly ILogger<JsonProcessingHistoryRepository> _logger;

    public JsonProcessingHistoryRepository(IOptions<OutlookOptions> options, ILogger<JsonProcessingHistoryRepository> logger, IApplicationConfiguration? configuration = null)
    {
        string configuredHistoryRoot = configuration is null
            ? string.Empty
            : string.IsNullOrWhiteSpace(configuration.ExecutionWorkspacePath)
                ? string.IsNullOrWhiteSpace(configuration.HistoryRoot)
                    ? Path.Combine(string.IsNullOrWhiteSpace(configuration.ArchiveRoot) ? configuration.RuntimeRoot : configuration.ArchiveRoot, "history")
                    : configuration.HistoryRoot
                : Path.Combine(configuration.ExecutionWorkspacePath, "history");
        _historyDirectory = ResolveConfiguredPath(configuredHistoryRoot);
        _logger = logger;
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        string filePath = GetFilePath(key);
        return Task.FromResult(File.Exists(filePath));
    }

    public async Task SaveAsync(ProcessedEmailRecord record, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(_historyDirectory);
        string filePath = GetFilePath(record.IncidentId);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(record, options), cancellationToken);
        _logger.LogInformation("Persisted processing history: {FilePath}", filePath);
    }

    private string GetFilePath(string key)
    {
        string safeKey = string.IsNullOrWhiteSpace(key) ? "unknown" : SanitizeKey(key);
        return Path.Combine(_historyDirectory, $"{safeKey}.json");
    }

    private static string SanitizeKey(string key)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        foreach (char invalidChar in invalidChars)
        {
            key = key.Replace(invalidChar, '_');
        }

        return key;
    }

    private static string ResolveConfiguredPath(string configuredPath)
    {
        if (string.IsNullOrWhiteSpace(configuredPath))
        {
            return Path.Combine(AppContext.BaseDirectory, "runtime", "archive", "history");
        }

        return Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, configuredPath));
    }
}
