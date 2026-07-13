using Microsoft.Extensions.Logging;
using VisualCron.Application.Outlook;
using VisualCron.Infrastructure.Outlook;

namespace VisualCron.Tests;

public sealed class FailureMailSubjectParserTests
{
    [Fact]
    public void Parse_ValidSubject_ReturnsExpectedMetadata()
    {
        var parser = new FailureMailSubjectParser(new FakeLogger<FailureMailSubjectParser>());

        FailureMailMetadata metadata = parser.Parse("[EXTERNAL] EAS-P5-MW - BAI6_File_Import - PROD - BHSIEAS32");

        Assert.True(metadata.IsValid);
        Assert.Equal("EXTERNAL", metadata.Prefix);
        Assert.Equal("BAI6_File_Import", metadata.JobName);
        Assert.Equal("PROD", metadata.Environment);
        Assert.Equal("BHSIEAS32", metadata.Server);
        Assert.Equal("[EXTERNAL] EAS-P5-MW - BAI6_File_Import - PROD - BHSIEAS32", metadata.OriginalSubject);
    }

    [Fact]
    public void Parse_EmptySubject_ReturnsInvalidMetadata()
    {
        var parser = new FailureMailSubjectParser(new FakeLogger<FailureMailSubjectParser>());

        FailureMailMetadata metadata = parser.Parse(string.Empty);

        Assert.False(metadata.IsValid);
        Assert.Contains("Subject is empty", metadata.ValidationMessage);
    }

    private sealed class FakeLogger<T> : ILogger<T>
    {
        public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;
        public bool IsEnabled(LogLevel logLevel) => false;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
    }

    private sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();
        public void Dispose() { }
    }
}
