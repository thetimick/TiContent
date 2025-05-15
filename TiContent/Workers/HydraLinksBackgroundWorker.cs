// ⠀
// HydraLinksBackgroundWorker.cs
// TiContent
// 
// Created by the_timick on 15.05.2025.
// ⠀

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TiContent.Components.Extensions;
using TiContent.DataSources;

namespace TiContent.Workers;

public class HydraLinksBackgroundWorker(IHydraLinksDataSource dataSource, ILogger<HydraLinksBackgroundWorker> logger): IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformationWithCaller("StartAsync");
        
        Task.Run(async () => await dataSource.ObtainItemsIfNeededAsync(), cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformationWithCaller("StopAsync");
        return Task.CompletedTask;
    }
}