using VisualCron.Application.Configuration;

namespace VisualCron.Tests;

public sealed class StartupValidationServiceTests
{
    [Fact]
    public void ValidateAndPrepare_CreatesRequiredDirectories_AndReturnsResolvedPaths()
    {
        string tempRoot = Path.Combine(Path.GetTempPath(), "startup-validation-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);

        try
        {
            var configuration = new TestApplicationConfiguration
            {
                RuntimeRoot = "runtime",
                ExecutionRoot = "execution",
                HistoryRoot = "history",
                ArchiveRoot = "archive",
                LogsRoot = "logs",
                PromptFileName = "Prompt.md",
                AIOutputFileName = "AIOutput.md",
                ReadMailbox = "user@example.com",
                DraftMailbox = "draft@example.com",
                CopilotRunnerScript = "RunCopilot.ps1",
                CopilotCommand = "pwsh"
            };

            File.WriteAllText(Path.Combine(tempRoot, "RunCopilot.ps1"), "Write-Host 'ok'");

            var validator = new StartupValidationService(configuration, tempRoot);

            StartupValidationResult result = validator.ValidateAndPrepare();

            Assert.Equal(Path.Combine(tempRoot, "runtime"), result.RuntimeRoot);
            Assert.Equal(Path.Combine(tempRoot, "execution"), result.ExecutionRoot);
            Assert.Equal(Path.Combine(tempRoot, "history"), result.HistoryRoot);
            Assert.Equal(Path.Combine(tempRoot, "archive"), result.ArchiveRoot);
            Assert.Equal(Path.Combine(tempRoot, "logs"), result.LogsRoot);
            Assert.True(Directory.Exists(result.RuntimeRoot));
            Assert.True(Directory.Exists(result.ExecutionRoot));
            Assert.True(Directory.Exists(result.HistoryRoot));
            Assert.True(Directory.Exists(result.ArchiveRoot));
            Assert.True(Directory.Exists(result.LogsRoot));
        }
        finally
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, recursive: true);
            }
        }
    }

    [Fact]
    public void ValidateAndPrepare_ThrowsWhenRequiredConfigurationIsMissing()
    {
        string tempRoot = Path.Combine(Path.GetTempPath(), "startup-validation-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);

        try
        {
            var configuration = new TestApplicationConfiguration
            {
                RuntimeRoot = "runtime",
                ExecutionRoot = "execution",
                HistoryRoot = "history",
                ArchiveRoot = "archive",
                LogsRoot = "logs",
                PromptFileName = "Prompt.md",
                AIOutputFileName = "AIOutput.md",
                ReadMailbox = string.Empty,
                DraftMailbox = "draft@example.com",
                CopilotRunnerScript = "RunCopilot.ps1",
                CopilotCommand = "pwsh"
            };

            var validator = new StartupValidationService(configuration, tempRoot);

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => validator.ValidateAndPrepare());
            Assert.Contains("ReadMailbox", exception.Message);
        }
        finally
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, recursive: true);
            }
        }
    }

    private sealed class TestApplicationConfiguration : IApplicationConfiguration
    {
        public string ApplicationName { get; set; } = string.Empty;

        public string RuntimeRoot { get; set; } = string.Empty;

        public string ExecutionRoot { get; set; } = string.Empty;

        public string ExecutionWorkspacePath { get; set; } = string.Empty;

        public string HistoryRoot { get; set; } = string.Empty;

        public string ArchiveRoot { get; set; } = string.Empty;

        public string LogsRoot { get; set; } = string.Empty;

        public string PromptFileName { get; set; } = string.Empty;

        public string AIOutputFileName { get; set; } = string.Empty;

        public string ReadMailbox { get; set; } = string.Empty;

        public string DraftMailbox { get; set; } = string.Empty;

        public string FailureMailPrefix { get; set; } = string.Empty;

        public string CopilotRunnerScript { get; set; } = string.Empty;

        public string CopilotCommand { get; set; } = string.Empty;
    }
}
