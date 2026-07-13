using VisualCron.Application.Outlook;

namespace VisualCron.Tests;

public sealed class ReadUnreadEmailsUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ReturnsReaderResults()
    {
        var reader = new StubUnreadEmailReader();
        var useCase = new ReadUnreadEmailsUseCase(reader);

        IReadOnlyList<UnreadEmailDto> result = await useCase.ExecuteAsync();

        Assert.Single(result);
        Assert.Equal("Test subject", result[0].Subject);
        Assert.Equal("Test sender", result[0].Sender);
    }

    private sealed class StubUnreadEmailReader : IUnreadEmailReader
    {
        public Task<IReadOnlyList<UnreadEmailDto>> ReadUnreadEmailsAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<UnreadEmailDto>>(
                new List<UnreadEmailDto>
                {
                    new() { Subject = "Test subject", Sender = "Test sender", ReceivedTime = DateTimeOffset.UtcNow }
                });
        }
    }
}