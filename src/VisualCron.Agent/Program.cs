using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VisualCron.Agent.Extensions;
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

        try
        {
            var logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("VisualCron.Agent");
            var parser = host.Services.GetRequiredService<IFailureMailSubjectParser>();
            FailureMailMetadata metadata = parser.Parse("[EXTERNAL] EAS-P5-MW - BAI6_File_Import - PROD - BHSIEAS32");
            logger.LogInformation("Failure mail parser sample result. JobName: {JobName}, Environment: {Environment}, Server: {Server}, IsValid: {IsValid}", metadata.JobName, metadata.Environment, metadata.Server, metadata.IsValid);

            var discoveryUseCase = host.Services.GetRequiredService<DiscoverFailureMailsUseCase>();
            FailureMailDiscoveryResult discoveryResult = await discoveryUseCase.ExecuteAsync(Guid.NewGuid().ToString("N"));
            logger.LogInformation("Failure mail discovery completed. New mails: {Count}", discoveryResult.NewMailCount);

            var attachmentUseCase = host.Services.GetRequiredService<IDownloadFailureMailAttachmentsUseCase>();
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
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Attachment download failed for mail subject: {Subject}", mail.Subject);
                }
            }
        }
        catch (Exception ex)
        {
            host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("VisualCron.Agent").LogError(ex, "Failure mail discovery failed.");
        }

        await host.StopAsync();
    }
}
