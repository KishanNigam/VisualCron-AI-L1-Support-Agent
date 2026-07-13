namespace VisualCron.Application.Outlook;

public interface IFailureMailSubjectParser
{
    FailureMailMetadata Parse(string? subject);
}
