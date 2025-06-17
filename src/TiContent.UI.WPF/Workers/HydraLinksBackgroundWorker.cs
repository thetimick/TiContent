// ⠀
// HydraLinksBackgroundWorker.cs
// TiContent.UI.WPF
//
// Created by the_timick on 15.05.2025.
// ⠀

using Microsoft.Extensions.Hosting;
using TiContent.UI.WPF.DataSources;

namespace TiContent.UI.WPF.Workers;

public class HydraLinksBackgroundWorker(IHydraLinksDataSource dataSource) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(async () => await dataSource.ObtainItemsIfNeededAsync(), cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}