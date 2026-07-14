namespace VisualCron.Application.Outlook;

public sealed class AIAnalysisResult
{
    public string FullContent { get; set; } = string.Empty;

    public string RawContent { get; set; } = string.Empty;

    public string IncidentSummary { get; set; } = string.Empty;

    public string RootCause { get; set; } = string.Empty;

    public string Impact { get; set; } = string.Empty;

    public string RecommendedAction { get; set; } = string.Empty;

    public string Confidence { get; set; } = string.Empty;

    public string ClientCommunication { get; set; } = string.Empty;

    public string InternalNotes { get; set; } = string.Empty;
}
