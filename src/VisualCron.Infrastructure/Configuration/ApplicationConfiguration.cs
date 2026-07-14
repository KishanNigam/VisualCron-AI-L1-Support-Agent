using Microsoft.Extensions.Options;
using VisualCron.Application.Configuration;

namespace VisualCron.Infrastructure.Configuration;

public sealed class ApplicationConfiguration : IApplicationConfiguration
{
    public ApplicationConfiguration(IOptions<ApplicationConfigurationOptions> options)
    {
        ApplicationName = options.Value.Application?.Name ?? string.Empty;
        RuntimeRoot = options.Value.Runtime?.Root ?? string.Empty;
        ExecutionRoot = options.Value.Runtime?.ExecutionRoot ?? string.Empty;
        ExecutionWorkspacePath = string.Empty;
        HistoryRoot = options.Value.Runtime?.HistoryRoot ?? string.Empty;
        ArchiveRoot = options.Value.Runtime?.ArchiveRoot ?? string.Empty;
        LogsRoot = options.Value.Runtime?.LogsRoot ?? string.Empty;
        PromptFileName = options.Value.Runtime?.PromptFileName ?? "Prompt.md";
        AIOutputFileName = options.Value.Runtime?.AIOutputFileName ?? "AIOutput.md";
        ReadMailbox = options.Value.Outlook?.ReadMailbox ?? string.Empty;
        DraftMailbox = options.Value.Outlook?.DraftMailbox ?? string.Empty;
        FailureMailPrefix = options.Value.Outlook?.FailureMailPrefix ?? string.Empty;
        CopilotRunnerScript = options.Value.Copilot?.RunnerScript ?? string.Empty;
        CopilotCommand = options.Value.Copilot?.Command ?? string.Empty;
    }

    public string ApplicationName { get; }

    public string RuntimeRoot { get; }

    public string ExecutionRoot { get; }

    public string ExecutionWorkspacePath { get; set; }

    public string HistoryRoot { get; }

    public string ArchiveRoot { get; }

    public string LogsRoot { get; }

    public string PromptFileName { get; }

    public string AIOutputFileName { get; }

    public string ReadMailbox { get; }

    public string DraftMailbox { get; }

    public string FailureMailPrefix { get; }

    public string CopilotRunnerScript { get; }

    public string CopilotCommand { get; }
}

public sealed class ApplicationConfigurationOptions
{
    public const string SectionName = "ApplicationConfiguration";

    public ApplicationSection? Application { get; set; }

    public RuntimeSection? Runtime { get; set; }

    public OutlookSection? Outlook { get; set; }

    public CopilotSection? Copilot { get; set; }
}

public sealed class ApplicationSection
{
    public string Name { get; set; } = string.Empty;
}

public sealed class RuntimeSection
{
    public string Root { get; set; } = string.Empty;

    public string ExecutionRoot { get; set; } = string.Empty;

    public string HistoryRoot { get; set; } = string.Empty;

    public string ArchiveRoot { get; set; } = string.Empty;

    public string LogsRoot { get; set; } = string.Empty;

    public string PromptFileName { get; set; } = "Prompt.md";

    public string AIOutputFileName { get; set; } = "AIOutput.md";
}

public sealed class OutlookSection
{
    public string ReadMailbox { get; set; } = string.Empty;

    public string DraftMailbox { get; set; } = string.Empty;

    public string FailureMailPrefix { get; set; } = string.Empty;
}

public sealed class CopilotSection
{
    public string RunnerScript { get; set; } = string.Empty;

    public string Command { get; set; } = string.Empty;
}
