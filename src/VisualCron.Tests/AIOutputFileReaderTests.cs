using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using VisualCron.Application.Outlook;
using VisualCron.Infrastructure.Outlook;

namespace VisualCron.Tests;

public sealed class AIOutputFileReaderTests
{
    [Fact]
    public async Task ReadAsync_WithExistingFile_ReturnsContentAndMetadata()
    {
        string filePath = CreateTempFile("AI_Output.md", "hello world", Encoding.UTF8);
        var reader = new AIOutputFileReader(NullLogger<AIOutputFileReader>.Instance);

        AIOutputReadResult result = await reader.ReadAsync(filePath);

        Assert.Equal(filePath, result.FilePath);
        Assert.Equal("hello world", result.FullContent);
        Assert.Equal(File.ReadAllBytes(filePath).Length, result.FileSize);
        Assert.Equal("hello world".Length, result.FullContent.Length);
        Assert.True(result.ReadTime > DateTimeOffset.MinValue);
    }

    [Fact]
    public async Task ReadAsync_WithMissingFile_ThrowsClearException()
    {
        string filePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "missing.md");
        var reader = new AIOutputFileReader(NullLogger<AIOutputFileReader>.Instance);

        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(() => reader.ReadAsync(filePath));

        Assert.Contains("AI output file", ex.Message);
        Assert.Contains("does not exist", ex.Message);
    }

    [Fact]
    public async Task ReadAsync_WithEmptyFile_ThrowsClearException()
    {
        string filePath = CreateTempFile("empty.md", string.Empty, Encoding.UTF8);
        var reader = new AIOutputFileReader(NullLogger<AIOutputFileReader>.Instance);

        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(() => reader.ReadAsync(filePath));

        Assert.Contains("empty", ex.Message);
    }

    private static string CreateTempFile(string fileName, string content, Encoding encoding)
    {
        string directory = Path.Combine(Path.GetTempPath(), "VisualCronAIOutputReaderTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        string filePath = Path.Combine(directory, fileName);
        File.WriteAllText(filePath, content, encoding);
        return filePath;
    }
}
