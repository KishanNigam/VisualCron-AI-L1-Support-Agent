using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VisualCron.Application.Configuration;
using VisualCron.Application.Outlook;
using VisualCron.Infrastructure.Configuration;
using VisualCron.Infrastructure.Outlook;

namespace VisualCron.Agent.Extensions;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OutlookOptions>(configuration.GetSection(OutlookOptions.SectionName));
        services.Configure<ApplicationConfigurationOptions>(configuration.GetSection(ApplicationConfigurationOptions.SectionName));
        services.AddSingleton<IApplicationConfiguration, ApplicationConfiguration>();
        services.AddSingleton<IOutlookMailboxService, OutlookMailboxService>();
        services.AddSingleton<IUnreadEmailReader, UnreadEmailReader>();
        services.AddSingleton<IFailureMailSubjectParser, FailureMailSubjectParser>();
        services.AddSingleton<IFailureMailDiscoveryService, FailureMailDiscoveryService>();
        services.AddSingleton<IFailureMailAttachmentDownloader, FailureMailAttachmentDownloader>();
        services.AddSingleton<ILogFileReader, LogFileReader>();
        services.AddSingleton<IProcessingHistoryRepository, JsonProcessingHistoryRepository>();
        return services;
    }
}
