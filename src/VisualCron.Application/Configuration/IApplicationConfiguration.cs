namespace VisualCron.Application.Configuration;

public interface IApplicationConfiguration
{
    string ApplicationName { get; }

    string RuntimeRoot { get; }

    string ExecutionRoot { get; }

    string ExecutionWorkspacePath { get; set; }

    string HistoryRoot { get; }

    string ArchiveRoot { get; }

    string LogsRoot { get; }

    string PromptFileName { get; }

    string AIOutputFileName { get; }

    string ReadMailbox { get; }

    string DraftMailbox { get; }

    string FailureMailPrefix { get; }

    string CopilotRunnerScript { get; }

    string CopilotCommand { get; }
}
