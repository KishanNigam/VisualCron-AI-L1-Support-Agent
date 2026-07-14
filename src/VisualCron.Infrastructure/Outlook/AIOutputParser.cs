using Microsoft.Extensions.Logging;
using VisualCron.Application.Outlook;

namespace VisualCron.Infrastructure.Outlook;

public sealed class AIOutputParser : IAIOutputParser
{
    private readonly ILogger<AIOutputParser> _logger;

    public AIOutputParser(ILogger<AIOutputParser> logger)
    {
        _logger = logger;
    }

    public AIAnalysisResult Parse(string content, string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(content);

        string normalizedContent = content.Trim();
        string normalizedFilePath = string.IsNullOrWhiteSpace(filePath) ? string.Empty : Path.GetFullPath(filePath);

        Dictionary<string, string> fields = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Incident Summary"] = ExtractSection(normalizedContent, "Incident Summary"),
            ["Root Cause"] = ExtractSection(normalizedContent, "Root Cause"),
            ["Impact"] = ExtractSection(normalizedContent, "Impact"),
            ["Recommended Action"] = ExtractSection(normalizedContent, "Recommended Action"),
            ["Confidence"] = ExtractSection(normalizedContent, "Confidence"),
            ["Client Communication"] = ExtractSection(normalizedContent, "Client Communication"),
            ["Internal Notes"] = ExtractSection(normalizedContent, "Internal Notes")
        };

        var result = new AIAnalysisResult
        {
            FullContent = content,
            RawContent = content,
            IncidentSummary = fields["Incident Summary"],
            RootCause = fields["Root Cause"],
            Impact = fields["Impact"],
            RecommendedAction = fields["Recommended Action"],
            Confidence = fields["Confidence"],
            ClientCommunication = fields["Client Communication"],
            InternalNotes = fields["Internal Notes"]
        };

        _logger.LogInformation("Parser completed for {FilePath}", normalizedFilePath);
        _logger.LogInformation("Fields found: IncidentSummary={IncidentSummary}; RootCause={RootCause}; Impact={Impact}; RecommendedAction={RecommendedAction}; Confidence={Confidence}; ClientCommunication={ClientCommunication}; InternalNotes={InternalNotes}", result.IncidentSummary, result.RootCause, result.Impact, result.RecommendedAction, result.Confidence, result.ClientCommunication, result.InternalNotes);
        return result;
    }

    private static string ExtractSection(string content, string heading)
    {
        string[] lines = content.Replace("\r\n", "\n").Split('\n');
        bool inSection = false;
        var collected = new List<string>();

        foreach (string line in lines)
        {
            string trimmed = line.Trim();
            if (trimmed.Equals($"## {heading}", StringComparison.OrdinalIgnoreCase) || trimmed.Equals($"### {heading}", StringComparison.OrdinalIgnoreCase))
            {
                inSection = true;
                continue;
            }

            if (inSection)
            {
                if (trimmed.StartsWith("## ", StringComparison.Ordinal) || trimmed.StartsWith("### ", StringComparison.Ordinal))
                {
                    break;
                }

                if (!string.IsNullOrWhiteSpace(trimmed))
                {
                    collected.Add(trimmed);
                }
            }
        }

        return string.Join("\n", collected).Trim();
    }
}
