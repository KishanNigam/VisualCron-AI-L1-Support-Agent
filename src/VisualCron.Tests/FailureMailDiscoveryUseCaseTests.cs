using Microsoft.Extensions.Logging.Abstractions;
using VisualCron.Application.Outlook;

namespace VisualCron.Tests;

public sealed class FailureMailDiscoveryUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ProcessesOnlyNewFailureMails()
    {
        var discoveryService = new StubDiscoveryService(
            new List<FailureMailDiscoveryItem>
            {
                new()
                {
                    IncidentId = "entry-1",
                    EntryId = "entry-1",
                    Subject = "[EXTERNAL] EAS-P5-MW - BAI6_File_Import - PROD - BHSIEAS32",
                    Sender = "ops@example.com",
                    ReceivedTime = DateTimeOffset.UtcNow
                },
                new()
                {
                    IncidentId = "entry-2",
                    EntryId = "entry-2",
                    Subject = "[EXTERNAL] EAS-P5-MW - BAI6_File_Import - PROD - BHSIEAS32",
                    Sender = "ops@example.com",
                    ReceivedTime = DateTimeOffset.UtcNow.AddMinutes(-5)
                }
            });

        var repository = new StubRepository(existingKeys: new[] { "entry-2" });
        var useCase = new DiscoverFailureMailsUseCase(discoveryService, repository, NullLogger<DiscoverFailureMailsUseCase>.Instance);

        FailureMailDiscoveryResult result = await useCase.ExecuteAsync("execution-1");

        Assert.Equal(2, result.InboxScanned);
        Assert.Equal(2, result.FailureMailsFound);
        Assert.Equal(1, result.AlreadyProcessedCount);
        Assert.Equal(1, result.NewMailCount);
        Assert.Equal(1, result.SkippedCount);
        Assert.Single(result.NewMails);
        Assert.Equal("entry-1", result.NewMails[0].IncidentId);
    }

    [Fact]
    public async Task ExecuteAsync_ProcessesSameSubjectDifferentReceivedTimeAsSeparateIncidents()
    {
        var discoveryService = new StubDiscoveryService(
            new List<FailureMailDiscoveryItem>
            {
                new()
                {
                    EntryId = "entry-1",
                    Subject = "[EXTERNAL] EAS-P5-MW - BAI6_File_Import - PROD - BHSIEAS32",
                    Sender = "ops@example.com",
                    ReceivedTime = DateTimeOffset.UtcNow
                },
                new()
                {
                    EntryId = "entry-2",
                    Subject = "[EXTERNAL] EAS-P5-MW - BAI6_File_Import - PROD - BHSIEAS32",
                    Sender = "ops@example.com",
                    ReceivedTime = DateTimeOffset.UtcNow.AddMinutes(-5)
                }
            });

        var repository = new StubRepository(existingKeys: Array.Empty<string>());
        var useCase = new DiscoverFailureMailsUseCase(discoveryService, repository, NullLogger<DiscoverFailureMailsUseCase>.Instance);

        FailureMailDiscoveryResult result = await useCase.ExecuteAsync("execution-2");

        Assert.Equal(2, result.NewMailCount);
        Assert.Equal(2, result.NewMails.Count);
        Assert.Equal(2, repository.SavedRecords.Count);
    }

    [Fact]
    public async Task ExecuteAsync_SkipsSameEntryId()
    {
        var discoveryService = new StubDiscoveryService(
            new List<FailureMailDiscoveryItem>
            {
                new()
                {
                    EntryId = "entry-1",
                    Subject = "[EXTERNAL] EAS-P5-MW - BAI6_File_Import - PROD - BHSIEAS32",
                    Sender = "ops@example.com",
                    ReceivedTime = DateTimeOffset.UtcNow
                },
                new()
                {
                    EntryId = "entry-1",
                    Subject = "[EXTERNAL] EAS-P5-MW - BAI6_File_Import - PROD - BHSIEAS32",
                    Sender = "ops@example.com",
                    ReceivedTime = DateTimeOffset.UtcNow.AddMinutes(-5)
                }
            });

        var repository = new StubRepository(existingKeys: Array.Empty<string>());
        var useCase = new DiscoverFailureMailsUseCase(discoveryService, repository, NullLogger<DiscoverFailureMailsUseCase>.Instance);

        FailureMailDiscoveryResult result = await useCase.ExecuteAsync("execution-3");

        Assert.Equal(1, result.NewMailCount);
        Assert.Equal(1, result.SkippedCount);
        Assert.Single(result.NewMails);
        Assert.Equal("entry-1", result.NewMails[0].EntryId);
    }

    private sealed class StubDiscoveryService : IFailureMailDiscoveryService
    {
        private readonly IReadOnlyList<FailureMailDiscoveryItem> _items;

        public StubDiscoveryService(IReadOnlyList<FailureMailDiscoveryItem> items)
        {
            _items = items;
        }

        public Task<IReadOnlyList<FailureMailDiscoveryItem>> DiscoverAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_items);
        }
    }

    private sealed class StubRepository : IProcessingHistoryRepository
    {
        private readonly HashSet<string> _existingKeys;

        public StubRepository(IEnumerable<string> existingKeys)
        {
            _existingKeys = new HashSet<string>(existingKeys, StringComparer.OrdinalIgnoreCase);
        }

        public List<ProcessedEmailRecord> SavedRecords { get; } = new();

        public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_existingKeys.Contains(key));
        }

        public Task SaveAsync(ProcessedEmailRecord record, CancellationToken cancellationToken = default)
        {
            _existingKeys.Add(record.IncidentId);
            SavedRecords.Add(record);
            return Task.CompletedTask;
        }
    }
}
