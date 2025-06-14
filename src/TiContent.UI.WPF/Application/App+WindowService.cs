using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TiContent.UI.WPF.Components.Converters;
using TiContent.UI.WPF.Components.Wrappers;
using TiContent.UI.WPF.Entities;
using TiContent.UI.WPF.Services.Storage;
using TiContent.UI.WPF.Windows.Main;

namespace TiContent.UI.WPF.Application;

public partial class App
{
    private class WindowService(IServiceProvider provider) : IHostedService, IDisposable
    {
        // Private Props

        private readonly AppDataBaseContext _db = provider.GetRequiredService<AppDataBaseContext>();

        private readonly IStorageService _storageService =
            provider.GetRequiredService<IStorageService>();

        private readonly MainWindow _mainWindow = provider.GetRequiredService<MainWindow>();

        // IHostedService

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _storageService.Obtain();
            if (_storageService.Cached is not { } storage)
                return Task.CompletedTask;

            // Устанавливаем тему
            ApplicationThemeManagerWrapper.Apply(storage.Window.ThemeIndex);

            if (!storage.Window.IsFirstLaunch())
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

            // Добавляем динамические ресурсы
            _mainWindow.Resources["StringToImageCacheConverter"] =
                new StringToImageCacheConverter();

            // Показываем окно
            _mainWindow.Show();

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _db.SaveChangesAsync(cancellationToken);
            _storageService.Save();
        }

        public void Dispose() { }
    }
}
