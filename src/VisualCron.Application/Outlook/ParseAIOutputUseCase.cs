using Microsoft.Extensions.Logging;

namespace VisualCron.Application.Outlook;

public sealed class ParseAIOutputUseCase
{
    private readonly IAIOutputParser _parser;
    private readonly ILogger<ParseAIOutputUseCase> _logger;

    public ParseAIOutputUseCase(IAIOutputParser parser, ILogger<ParseAIOutputUseCase> logger)
    {
        _parser = parser;
        _logger = logger;
    }

    public AIAnalysisResult Execute(string content, string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(content);

        _logger.LogInformation("Parser Started for {FilePath}", filePath);
        AIAnalysisResult result = _parser.Parse(content, filePath);
        _logger.LogInformation("Parser Completed for {FilePath}", filePath);
        _logger.LogInformation("Fields Found: IncidentSummary={IncidentSummary}; RootCause={RootCause}; Impact={Impact}; RecommendedAction={RecommendedAction}; Confidence={Confidence}; ClientCommunication={ClientCommunication}; InternalNotes={InternalNotes}", result.IncidentSummary, result.RootCause, result.Impact, result.RecommendedAction, result.Confidence, result.ClientCommunication, result.InternalNotes);
        return result;
    }
}
