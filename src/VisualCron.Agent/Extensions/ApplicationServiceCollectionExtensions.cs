using Microsoft.Extensions.DependencyInjection;
using VisualCron.Application.Outlook;

namespace VisualCron.Agent.Extensions;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddSingleton<ReadUnreadEmailsUseCase>();
        services.AddSingleton<DiscoverFailureMailsUseCase>();
        services.AddSingleton<DownloadFailureMailAttachmentsUseCase>();
        services.AddSingleton<IDownloadFailureMailAttachmentsUseCase, DownloadFailureMailAttachmentsUseCase>();
        services.AddSingleton<ReadLogFilesUseCase>();
        services.AddSingleton<ReadAIOutputFileUseCase>();
        services.AddSingleton<ParseAIOutputUseCase>();
        return services;
    }
}
