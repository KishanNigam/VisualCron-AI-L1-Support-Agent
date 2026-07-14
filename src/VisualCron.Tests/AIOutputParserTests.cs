using Microsoft.Extensions.Logging.Abstractions;
using VisualCron.Application.Outlook;
using VisualCron.Infrastructure.Outlook;

namespace VisualCron.Tests;

public sealed class AIOutputParserTests
{
    [Fact]
    public void Parse_WithStructuredSections_ExtractsFieldsAndPreservesContent()
    {
        const string content = """
# AI Analysis

## Incident Summary
Service outage on primary API.

## Root Cause
Database pool exhausted.

## Impact
Customers could not submit orders.

## Recommended Action
Restart the pool and scale the service.

## Confidence
High

## Client Communication
We are investigating and will update you shortly.

## Internal Notes
Escalated to platform team.
""";

        var parser = new AIOutputParser(NullLogger<AIOutputParser>.Instance);

        AIAnalysisResult result = parser.Parse(content, "C:/temp/AI_Output.md");

        Assert.Equal(content, result.FullContent);
        Assert.Equal(content, result.RawContent);
        Assert.Equal("Service outage on primary API.", result.IncidentSummary);
        Assert.Equal("Database pool exhausted.", result.RootCause);
        Assert.Equal("Customers could not submit orders.", result.Impact);
        Assert.Equal("Restart the pool and scale the service.", result.RecommendedAction);
        Assert.Equal("High", result.Confidence);
        Assert.Equal("We are investigating and will update you shortly.", result.ClientCommunication);
        Assert.Equal("Escalated to platform team.", result.InternalNotes);
    }

    [Fact]
    public void Parse_WithMissingFields_LeavesThemEmpty()
    {
        const string content = "# AI Analysis\n\nNo structured sections were provided.";

        var parser = new AIOutputParser(NullLogger<AIOutputParser>.Instance);

        AIAnalysisResult result = parser.Parse(content, "C:/temp/AI_Output.md");

        Assert.Equal(string.Empty, result.IncidentSummary);
        Assert.Equal(string.Empty, result.RootCause);
        Assert.Equal(string.Empty, result.Impact);
        Assert.Equal(string.Empty, result.RecommendedAction);
        Assert.Equal(string.Empty, result.Confidence);
        Assert.Equal(string.Empty, result.ClientCommunication);
        Assert.Equal(string.Empty, result.InternalNotes);
        Assert.Equal(content, result.RawContent);
    }
}
