// ⠀
// StorageHostedService.cs
// TiContent.Avalonia
// 
// Created by the_timick on 21.05.2025.
// ⠀

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using TiContent.Avalonia.Services.Storage;

namespace TiContent.Avalonia.Services.Hosted;

public class StorageHostedService(IStorageService storage): IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        storage.Obtain();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        storage.Save();
        return Task.CompletedTask;
    }
}