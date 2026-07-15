using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VisualCron.Agent.Extensions;
using VisualCron.Application.Configuration;
using VisualCron.Application.Outlook;

namespace VisualCron.Agent;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        using IHost host = Host.CreateDefaultBuilder(args)
            .UseContentRoot(AppContext.BaseDirectory)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(AppContext.BaseDirectory);
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
                config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: false);
                config.AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                services.AddApplicationLayer();
                services.AddInfrastructureLayer(context.Configuration);
                services.AddSharedLayer();
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSimpleConsole();
            })
            .Build();

        await host.StartAsync();

        ILogger logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("VisualCron.Agent");

        try
        {
            var configuration = host.Services.GetRequiredService<IApplicationConfiguration>();
            var startupValidator = new StartupValidationService(configuration, AppContext.BaseDirectory);
            StartupValidationResult validation = startupValidator.ValidateAndPrepare();
            string executionRoot = validation.ExecutionRoot;
            string executionWorkspacePath = Path.Combine(executionRoot, DateTime.UtcNow.ToString("yyyyMMdd_HHmmss"));
            Directory.CreateDirectory(executionWorkspacePath);
            configuration.ExecutionWorkspacePath = executionWorkspacePath;
            logger.LogInformation("Execution Root: {ExecutionRoot}", executionRoot);
            logger.LogInformation("Execution Workspace: {ExecutionWorkspace}", executionWorkspacePath);

            var parser = host.Services.GetRequiredService<IFailureMailSubjectParser>();
            var discoveryUseCase = host.Services.GetRequiredService<DiscoverFailureMailsUseCase>();
            string executionId = Guid.NewGuid().ToString("N");
            FailureMailDiscoveryResult discoveryResult = await discoveryUseCase.ExecuteAsync(executionId);
            logger.LogInformation("Failure mail discovery completed. New mails: {Count}", discoveryResult.NewMailCount);

            var attachmentUseCase = host.Services.GetRequiredService<IDownloadFailureMailAttachmentsUseCase>();
            bool promptGenerated = false;
            foreach (FailureMailDiscoveryItem mail in discoveryResult.NewMails)
            {
                logger.LogInformation("Mail Subject: {Subject}", mail.Subject);

                if (mail.MailItem is null)
                {
                    logger.LogWarning("No mail item available for attachment download. Subject: {Subject}", mail.Subject);
                    continue;
                }

                try
                {
                    IReadOnlyList<DownloadedAttachment> attachments = await attachmentUseCase.ExecuteAsync(mail.MailItem);
                    logger.LogInformation("Attachment Count: {Count}", attachments.Count);

                    if (attachments.Count == 0)
                    {
                        logger.LogInformation("No attachments found for mail subject: {Subject}", mail.Subject);
                        continue;
                    }

                    string? downloadFolder = attachments
                        .Select(attachment => Path.GetDirectoryName(attachment.FullPath))
                        .FirstOrDefault(directory => !string.IsNullOrWhiteSpace(directory));

                    logger.LogInformation("Download Folder: {Folder}", downloadFolder ?? string.Empty);

                    var logReader = host.Services.GetRequiredService<ReadLogFilesUseCase>();
                    var logFiles = attachments
                        .Where(attachment => string.Equals(Path.GetExtension(attachment.FileName), ".log", StringComparison.OrdinalIgnoreCase))
                        .Select(attachment => attachment.FullPath)
                        .Where(path => !string.IsNullOrWhiteSpace(path))
                        .ToList();

                    if (logFiles.Count == 0)
                    {
                        logger.LogInformation("No log files found for mail subject: {Subject}", mail.Subject);
                        continue;
                    }

                    IReadOnlyList<LogFileReadResult> logResults = await logReader.ExecuteAsync(logFiles);
                    foreach (LogFileReadResult result in logResults)
                    {
                        logger.LogInformation("File Name: {FileName}", result.LogFileName);
                        logger.LogInformation("Line Count: {LineCount}", result.LineCount);
                        logger.LogInformation("File Size: {FileSize}", result.FileSize);

                        string[] firstThreeLines = result.Content
                            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                            .Take(3)
                            .ToArray();

                        for (int index = 0; index < firstThreeLines.Length; index++)
                        {
                            logger.LogInformation("Line {Index}: {Line}", index + 1, firstThreeLines[index]);
                        }

                        FailureMailMetadata mailMetadata = parser.Parse(mail.Subject ?? string.Empty);
                        string? promptDirectory = Path.GetDirectoryName(result.FullPath);
                        if (!string.IsNullOrWhiteSpace(promptDirectory))
                        {
                            string promptFileName = string.IsNullOrWhiteSpace(configuration.PromptFileName) ? "Prompt.md" : configuration.PromptFileName;
                            string promptFilePath = Path.Combine(promptDirectory, promptFileName);
                            string promptContent = $$"""
# VisualCron AI Production Support Agent

## INCIDENT DETAILS

Execution ID: {{executionId}}
Subject: {{mail.Subject ?? "Not enough evidence"}}
Job Name: {{mailMetadata.JobName ?? "Not enough evidence"}}
Environment: {{mailMetadata.Environment ?? "Not enough evidence"}}
Server: {{mailMetadata.Server ?? "Not enough evidence"}}
Received Time: {{mail.ReceivedTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "Not enough evidence"}}
Sender: {{mail.Sender ?? "Not enough evidence"}}

## LOG FILES

### {{result.LogFileName}}
{{result.Content}}

## YOUR TASK

You are an L3 Production Support Engineer.

Analyze the logs.

Generate

1 Failure Summary
2 Root Cause
3 Failed Component
4 Evidence
5 Business Impact
6 Recommended Fix
7 Client Acknowledgement Mail
8 Client RCA Mail

Return markdown.

Do not hallucinate.

If information is missing,
say "Not enough evidence".
""";

                            File.WriteAllText(promptFilePath, promptContent);
                            long promptSize = new FileInfo(promptFilePath).Length;
                            logger.LogInformation("Prompt Generated: {Path}", promptFilePath);
                            logger.LogInformation("Prompt Path: {Path}", promptFilePath);
                            logger.LogInformation("Prompt Size: {Size} bytes", promptSize);
                            logger.LogInformation("Workflow completed after Prompt.md generation.");
                            promptGenerated = true;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Attachment download failed for mail subject: {Subject}", mail.Subject);
                }

                if (promptGenerated)
                {
                    break;
                }
            }
        }
        catch (InvalidOperationException ex)
        {
            logger.LogCritical(ex, "Startup validation failed.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failure mail discovery failed.");
        }

        await host.StopAsync();
    }
}
