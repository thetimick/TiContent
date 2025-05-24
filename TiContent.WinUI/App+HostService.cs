// ㅤ
// App.HostService.cs
// TiContent.WinUI
// ㅤ
// Created by Timick on 16.12.2024.
// ㅤ

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using System;
using System.Threading;
using System.Threading.Tasks;
using TiContent.WinUI.Services.Storage;
using TiContent.WinUI.UI.Windows.Main;

namespace TiContent.WinUI;

public partial class App
{
    private class AppHostService(IServiceProvider provider) : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            provider.GetRequiredService<IStorageService>()
                .Obtain();
                
            var window = provider.GetRequiredService<MainWindow>();
            ConfigureWindow(window);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            provider.GetRequiredService<IStorageService>()
                .Save();
                
            return Task.CompletedTask;
        }

        private static void ConfigureWindow(Window window)
        {
            window.Activate();
            window.Closed += async (_, _) => await AppHost.StopAsync();
        }
    }
}