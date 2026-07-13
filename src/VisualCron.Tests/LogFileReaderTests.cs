using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using VisualCron.Application.Outlook;
using VisualCron.Infrastructure.Outlook;

namespace VisualCron.Tests;

public sealed class LogFileReaderTests
{
    [Fact]
    public async Task ReadAsync_WithSingleLogFile_ReturnsFullContentAndMetadata()
    {
        string filePath = CreateTempFile("single.log", "line one\nline two\n", Encoding.UTF8);
        var reader = new LogFileReader(NullLogger<LogFileReader>.Instance);

        IReadOnlyList<LogFileReadResult> result = await reader.ReadAsync(new[] { filePath });

        Assert.Single(result);
        Assert.Equal("single.log", result[0].LogFileName);
        Assert.Equal(filePath, result[0].FullPath);
        Assert.Equal("line one\nline two\n", result[0].Content);
        Assert.Equal(2, result[0].LineCount);
        Assert.True(result[0].FileSize > 0);
    }

    [Fact]
    public async Task ReadAsync_WithMultipleLogFiles_ReadsEachFile()
    {
        string firstPath = CreateTempFile("first.log", "alpha\n", Encoding.UTF8);
        string secondPath = CreateTempFile("second.log", "beta\ngamma\n", Encoding.UTF8);
        var reader = new LogFileReader(NullLogger<LogFileReader>.Instance);

        IReadOnlyList<LogFileReadResult> result = await reader.ReadAsync(new[] { firstPath, secondPath });

        Assert.Equal(2, result.Count);
        Assert.Equal(new[] { "first.log", "second.log" }, result.Select(x => x.LogFileName).ToArray());
    }

    [Fact]
    public async Task ReadAsync_WithEmptyLogFile_ReturnsEmptyContentAndZeroLines()
    {
        string filePath = CreateTempFile("empty.log", string.Empty, Encoding.UTF8);
        var reader = new LogFileReader(NullLogger<LogFileReader>.Instance);

        IReadOnlyList<LogFileReadResult> result = await reader.ReadAsync(new[] { filePath });

        Assert.Single(result);
        Assert.Equal(string.Empty, result[0].Content);
        Assert.Equal(0, result[0].LineCount);
    }

    [Fact]
    public async Task ReadAsync_WithLargeLogFile_PreservesFullContent()
    {
        string content = string.Concat(Enumerable.Repeat("0123456789\n", 2000));
        string filePath = CreateTempFile("large.log", content, Encoding.UTF8);
        var reader = new LogFileReader(NullLogger<LogFileReader>.Instance);

        IReadOnlyList<LogFileReadResult> result = await reader.ReadAsync(new[] { filePath });

        Assert.Single(result);
        Assert.Equal(content, result[0].Content);
        Assert.Equal(2000, result[0].LineCount);
    }

    [Fact]
    public async Task ReadAsync_WithExecutionDirectory_ReturnsAllLogFilesAndExecutionFolder()
    {
        string directory = Path.Combine(Path.GetTempPath(), "VisualCronLogReaderTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        string firstPath = CreateTempFileInDirectory(directory, "VisualCron.log", "alpha\n", Encoding.UTF8);
        string secondPath = CreateTempFileInDirectory(directory, "Batch.log", "beta\ngamma\n", Encoding.UTF8);
        string nonLogPath = CreateTempFileInDirectory(directory, "notes.txt", "ignore me", Encoding.UTF8);
        var reader = new LogFileReader(NullLogger<LogFileReader>.Instance);

        IReadOnlyList<LogFileReadResult> result = await reader.ReadAsync(new[] { directory });

        Assert.Equal(2, result.Count);
        Assert.All(result, item => Assert.Equal(directory, item.ExecutionFolder));
        Assert.Equal(new[] { "Batch.log", "VisualCron.log" }, result.Select(x => x.LogFileName).OrderBy(x => x).ToArray());
        Assert.Equal(firstPath, result.Single(x => x.LogFileName == "VisualCron.log").FullPath);
        Assert.Equal(secondPath, result.Single(x => x.LogFileName == "Batch.log").FullPath);
        Assert.DoesNotContain(result, item => item.LogFileName == "notes.txt");
    }

    private static string CreateTempFile(string fileName, string content, Encoding encoding)
    {
        string directory = Path.Combine(Path.GetTempPath(), "VisualCronLogReaderTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        return CreateTempFileInDirectory(directory, fileName, content, encoding);
    }

    private static string CreateTempFileInDirectory(string directory, string fileName, string content, Encoding encoding)
    {
        string filePath = Path.Combine(directory, fileName);
        File.WriteAllText(filePath, content, encoding);
        return filePath;
    }
}
