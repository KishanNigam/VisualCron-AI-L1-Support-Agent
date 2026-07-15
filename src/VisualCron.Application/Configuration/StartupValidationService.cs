using System.Diagnostics;

namespace VisualCron.Application.Configuration;

public sealed class StartupValidationService
{
    private readonly IApplicationConfiguration _configuration;
    private readonly string _contentRoot;

    public StartupValidationService(IApplicationConfiguration configuration, string? contentRoot = null)
    {
        _configuration = configuration;
        _contentRoot = string.IsNullOrWhiteSpace(contentRoot) ? AppContext.BaseDirectory : contentRoot;
    }

    public StartupValidationResult ValidateAndPrepare()
    {
        ValidateRequiredConfiguration();

        string runtimeRoot = ResolveConfiguredPath(_configuration.RuntimeRoot);
        string executionRoot = ResolveConfiguredPath(_configuration.ExecutionRoot);
        string historyRoot = ResolveConfiguredPath(_configuration.HistoryRoot);
        string archiveRoot = ResolveConfiguredPath(_configuration.ArchiveRoot);
        string logsRoot = ResolveConfiguredPath(_configuration.LogsRoot);

        foreach (string directory in new[] { runtimeRoot, executionRoot, historyRoot, archiveRoot, logsRoot })
        {
            Directory.CreateDirectory(directory);
        }

        return new StartupValidationResult(runtimeRoot, executionRoot, historyRoot, archiveRoot, logsRoot, string.Empty);
    }

    private void ValidateRequiredConfiguration()
    {
        ValidateRequiredValue(_configuration.ReadMailbox, nameof(IApplicationConfiguration.ReadMailbox));
        ValidateRequiredValue(_configuration.DraftMailbox, nameof(IApplicationConfiguration.DraftMailbox));
        ValidateRequiredValue(_configuration.RuntimeRoot, nameof(IApplicationConfiguration.RuntimeRoot));
        ValidateRequiredValue(_configuration.ExecutionRoot, nameof(IApplicationConfiguration.ExecutionRoot));
        ValidateRequiredValue(_configuration.HistoryRoot, nameof(IApplicationConfiguration.HistoryRoot));
        ValidateRequiredValue(_configuration.ArchiveRoot, nameof(IApplicationConfiguration.ArchiveRoot));
        ValidateRequiredValue(_configuration.LogsRoot, nameof(IApplicationConfiguration.LogsRoot));
        ValidateRequiredValue(_configuration.PromptFileName, nameof(IApplicationConfiguration.PromptFileName));
    }

    private static void ValidateRequiredValue(string value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Startup validation failed: {propertyName} is required.");
        }
    }

    private string ResolveConfiguredPath(string configuredPath)
    {
        if (string.IsNullOrWhiteSpace(configuredPath))
        {
            return Path.Combine(_contentRoot, "runtime");
        }

        return Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.GetFullPath(Path.Combine(_contentRoot, configuredPath));
    }

    private string ResolveExistingPath(string configuredPath)
    {
        if (string.IsNullOrWhiteSpace(configuredPath))
        {
            return string.Empty;
        }

        if (Path.IsPathRooted(configuredPath))
        {
            return configuredPath;
        }

        string[] candidatePaths =
        [
            Path.GetFullPath(Path.Combine(_contentRoot, configuredPath)),
            Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), configuredPath))
        ];

        foreach (string candidatePath in candidatePaths.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (File.Exists(candidatePath))
            {
                return candidatePath;
            }
        }

        return candidatePaths[0];
    }

    private static bool CanResolveCommand(string command)
    {
        try
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = command,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using Process? process = Process.Start(startInfo);
            if (process is null)
            {
                return false;
            }

            process.WaitForExit(2000);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

public sealed record StartupValidationResult(
    string RuntimeRoot,
    string ExecutionRoot,
    string HistoryRoot,
    string ArchiveRoot,
    string LogsRoot,
    string CopilotRunnerScript);
