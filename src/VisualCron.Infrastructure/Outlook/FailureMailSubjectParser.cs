using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using VisualCron.Application.Outlook;

namespace VisualCron.Infrastructure.Outlook;

public sealed class FailureMailSubjectParser : IFailureMailSubjectParser
{
    private static readonly Regex SubjectPattern = new(
        @"^\[(?<prefix>[^\]]+)\]\s*(?:.+?)\s-\s(?<job>.+?)\s-\s(?<environment>.+?)\s-\s(?<server>.+)$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private readonly ILogger<FailureMailSubjectParser> _logger;

    public FailureMailSubjectParser(ILogger<FailureMailSubjectParser> logger)
    {
        _logger = logger;
    }

    public FailureMailMetadata Parse(string? subject)
    {
        var metadata = new FailureMailMetadata
        {
            OriginalSubject = subject ?? string.Empty
        };

        try
        {
            if (string.IsNullOrWhiteSpace(subject))
            {
                metadata.IsValid = false;
                metadata.ValidationMessage = "Subject is empty.";
                LogResult(metadata);
                return metadata;
            }

            string trimmed = subject.Trim();
            Match match = SubjectPattern.Match(trimmed);
            if (!match.Success)
            {
                metadata.IsValid = false;
                metadata.ValidationMessage = "Subject format is invalid. Expected format: [PREFIX] <ignored> - JOB - ENV - SERVER.";
                LogResult(metadata);
                return metadata;
            }

            string prefix = match.Groups["prefix"].Value.Trim();
            string jobName = match.Groups["job"].Value.Trim();
            string environment = match.Groups["environment"].Value.Trim();
            string server = match.Groups["server"].Value.Trim();

            if (string.IsNullOrWhiteSpace(prefix) || string.IsNullOrWhiteSpace(jobName) || string.IsNullOrWhiteSpace(environment) || string.IsNullOrWhiteSpace(server))
            {
                metadata.IsValid = false;
                metadata.ValidationMessage = "One or more required parts are missing.";
                LogResult(metadata);
                return metadata;
            }

            metadata.Prefix = prefix;
            metadata.JobName = jobName;
            metadata.Environment = environment;
            metadata.Server = server;
            metadata.IsValid = true;
            metadata.ValidationMessage = "Valid failure mail subject.";

            LogResult(metadata);
            return metadata;
        }
        catch (Exception ex)
        {
            metadata.IsValid = false;
            metadata.ValidationMessage = $"Parsing failed: {ex.Message}";
            _logger.LogError(ex, "Original Subject: {Subject}", metadata.OriginalSubject);
            _logger.LogError(ex, "Validation Result: {Result} - {Message}", metadata.IsValid, metadata.ValidationMessage);
            return metadata;
        }
    }

    private void LogResult(FailureMailMetadata metadata)
    {
        _logger.LogInformation("Original Subject: {Subject}", metadata.OriginalSubject);
        _logger.LogInformation("Validation Result: {Result} - {Message}", metadata.IsValid, metadata.ValidationMessage);
        _logger.LogInformation("Job Name: {JobName}", metadata.JobName);
        _logger.LogInformation("Environment: {Environment}", metadata.Environment);
        _logger.LogInformation("Server: {Server}", metadata.Server);
    }
}
