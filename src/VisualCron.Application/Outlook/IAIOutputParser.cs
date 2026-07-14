namespace VisualCron.Application.Outlook;

public interface IAIOutputParser
{
    AIAnalysisResult Parse(string content, string filePath);
}
