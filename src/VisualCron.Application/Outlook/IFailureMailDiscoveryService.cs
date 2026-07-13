namespace VisualCron.Application.Outlook;

public interface IFailureMailDiscoveryService
{
    Task<IReadOnlyList<FailureMailDiscoveryItem>> DiscoverAsync(CancellationToken cancellationToken = default);
}
