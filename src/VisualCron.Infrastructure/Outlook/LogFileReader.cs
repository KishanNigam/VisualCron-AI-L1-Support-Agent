using System.Text;
using Microsoft.Extensions.Logging;
using VisualCron.Application.Outlook;

namespace VisualCron.Infrastructure.Outlook;

public sealed class LogFileReader : ILogFileReader
{
    private readonly ILogger<LogFileReader> _logger;

    public LogFileReader(ILogger<LogFileReader> logger)
    {
        _logger = logger;
    }

    public Task<IReadOnlyList<LogFileReadResult>> ReadAsync(IEnumerable<string> filePaths, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filePaths);

        var results = new List<LogFileReadResult>();
        foreach (string? inputPath in filePaths.Where(path => !string.IsNullOrWhiteSpace(path)))
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                string normalizedPath = Path.GetFullPath(inputPath!);
                if (Directory.Exists(normalizedPath))
                {
                    string executionFolder = normalizedPath;
                    IEnumerable<string> logFiles = Directory.EnumerateFiles(executionFolder, "*.log", SearchOption.TopDirectoryOnly)
                        .Where(path => string.Equals(Path.GetExtension(path), ".log", StringComparison.OrdinalIgnoreCase))
                        .OrderBy(path => path, StringComparer.OrdinalIgnoreCase);

                    _logger.LogInformation("Execution Folder: {ExecutionFolder}", executionFolder);
                    foreach (string logFilePath in logFiles)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        ReadAndAddResult(results, logFilePath, executionFolder, cancellationToken);
                    }

                    continue;
                }

                if (!File.Exists(normalizedPath))
                {
                    _logger.LogWarning("Log file missing: {FilePath}", normalizedPath);
                    continue;
                }

                string extension = Path.GetExtension(normalizedPath);
                if (!string.Equals(extension, ".log", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                ReadAndAddResult(results, normalizedPath, Path.GetDirectoryName(normalizedPath) ?? normalizedPath, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read log file: {FilePath}", inputPath);
            }
        }

        return Task.FromResult<IReadOnlyList<LogFileReadResult>>(results);
    }

    private void ReadAndAddResult(List<LogFileReadResult> results, string fullPath, string executionFolder, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Reading file: {FilePath}", fullPath);
            FileInfo fileInfo = new(fullPath);
            string content = ReadFileContent(fullPath);
            int lineCount = CountLines(content);

            _logger.LogInformation("Completed File: {FilePath} Line Count: {LineCount} File Size: {FileSize}", fullPath, lineCount, fileInfo.Length);

            results.Add(new LogFileReadResult
            {
                LogFileName = Path.GetFileName(fullPath),
                FullPath = fullPath,
                ExecutionFolder = executionFolder,
                Content = content,
                LineCount = lineCount,
                FileSize = fileInfo.Length
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read log file: {FilePath}", fullPath);
        }
    }

    private static string ReadFileContent(string filePath)
    {
        byte[] buffer = File.ReadAllBytes(filePath);

        return ReadWithFallbackEncoding(buffer);
    }

    private static string ReadWithFallbackEncoding(byte[] buffer)
    {
        foreach (Encoding encoding in GetCandidateEncodings())
        {
            try
            {
                return encoding.GetString(buffer);
            }
            catch
            {
                // Ignore and fall back to another encoding.
            }
        }

        return Encoding.UTF8.GetString(buffer);
    }

    private static IEnumerable<Encoding> GetCandidateEncodings()
    {
        yield return new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        yield return new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
        yield return Encoding.GetEncoding(1252);
        yield return Encoding.ASCII;
    }

    private static int CountLines(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return 0;
        }

        int lineCount = 0;
        using var reader = new StringReader(content);
        while (reader.ReadLine() is not null)
        {
            lineCount++;
        }

        return lineCount;
    }
}
