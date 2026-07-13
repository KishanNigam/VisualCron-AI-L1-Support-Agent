using Microsoft.Extensions.Logging.Abstractions;
using VisualCron.Application.Outlook;
using VisualCron.Infrastructure.Outlook;

namespace VisualCron.Tests;

public class FailureMailAttachmentDownloaderTests
{
    [Fact]
    public async Task DownloadAsync_WithSingleLogAttachment_ReturnsDownloadedAttachment()
    {
        var downloader = new FailureMailAttachmentDownloader(NullLogger<FailureMailAttachmentDownloader>.Instance);
        var mail = new FakeMailItem(new[] { new FakeAttachment("error.log") });

        IReadOnlyList<DownloadedAttachment> result = await downloader.DownloadAsync(mail);

        Assert.Single(result);
        Assert.Equal("error.log", result[0].FileName);
        Assert.True(File.Exists(result[0].FullPath));
        Assert.Equal(".log", result[0].Extension);
    }

    [Fact]
    public async Task DownloadAsync_WithMultipleLogAttachments_ReturnsAllDownloads()
    {
        var downloader = new FailureMailAttachmentDownloader(NullLogger<FailureMailAttachmentDownloader>.Instance);
        var mail = new FakeMailItem(new[]
        {
            new FakeAttachment("one.log"),
            new FakeAttachment("two.log")
        });

        IReadOnlyList<DownloadedAttachment> result = await downloader.DownloadAsync(mail);

        Assert.Equal(2, result.Count);
        Assert.All(result, attachment => Assert.Equal(".log", attachment.Extension));
    }

    [Fact]
    public async Task DownloadAsync_WithMixedAttachments_SkipsNonLogFiles()
    {
        var downloader = new FailureMailAttachmentDownloader(NullLogger<FailureMailAttachmentDownloader>.Instance);
        var mail = new FakeMailItem(new object[]
        {
            new FakeAttachment("error.log"),
            new FakeAttachment("notes.txt"),
            new FakeAttachment("trace.log")
        });

        IReadOnlyList<DownloadedAttachment> result = await downloader.DownloadAsync(mail);

        Assert.Equal(2, result.Count);
        Assert.Equal(new[] { "error.log", "trace.log" }, result.Select(x => x.FileName).ToArray());
    }

    [Fact]
    public async Task DownloadAsync_WithNoAttachments_ReturnsEmptyList()
    {
        var downloader = new FailureMailAttachmentDownloader(NullLogger<FailureMailAttachmentDownloader>.Instance);
        var mail = new FakeMailItem(Array.Empty<object>());

        IReadOnlyList<DownloadedAttachment> result = await downloader.DownloadAsync(mail);

        Assert.Empty(result);
    }

    private sealed class FakeMailItem
    {
        public FakeMailItem(IEnumerable<object> attachments)
        {
            Attachments = new FakeAttachmentsCollection(attachments);
        }

        public FakeAttachmentsCollection Attachments { get; }
    }

    private sealed class FakeAttachmentsCollection
    {
        private readonly IReadOnlyList<object> _attachments;

        public FakeAttachmentsCollection(IEnumerable<object> attachments)
        {
            _attachments = attachments.ToList();
        }

        public int Count => _attachments.Count;

        public object Item(int index) => _attachments[index - 1];
    }

    private sealed class FakeAttachment
    {
        public FakeAttachment(string fileName)
        {
            FileName = fileName;
        }

        public string FileName { get; }

        public void SaveAsFile(string destinationPath)
        {
            File.WriteAllText(destinationPath, "test content");
        }
    }
}
