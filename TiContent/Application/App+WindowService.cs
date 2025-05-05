using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TiContent.Components.Wrappers;
using TiContent.Services.Storage;
using TiContent.Windows;

namespace TiContent.Application;

public partial class App
{
    private class WindowService(
        IServiceProvider provider
    ) : IHostedService, IDisposable {
        
        private readonly IStorageService _storageService = provider.GetRequiredService<IStorageService>();
        private readonly MainWindow _mainWindow = provider.GetRequiredService<MainWindow>();
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (_storageService.Cached is not { } storage)
                return;

            // Устанавливаем тему
            ApplicationThemeManagerWrapper.Apply(storage.Window.ThemeIndex);

            if (!storage.Window.IsFirstLaunch)
            {
                // Устанавливаем размер окна
                if (storage.Window.IsWindowSizePersistent)
                {
                    _mainWindow.Width = storage.Window.Width ?? 0;
                    _mainWindow.Height = storage.Window.Height ?? 0;
                }

                // Устанавливаем позицию окна
                if (storage.Window.IsWindowOnCenterScreen)
                {
                    _mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }
                else
                {
                    _mainWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                    _mainWindow.Left = storage.Window.X ?? 0;
                    _mainWindow.Top = storage.Window.Y ?? 0;
                }
            }

            // Показываем окно
            await Current.Dispatcher.InvokeAsync(() => _mainWindow.Show());
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        { }
    }
}