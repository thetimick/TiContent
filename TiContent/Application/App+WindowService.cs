using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TiContent.Components.Abstractions;
using TiContent.Components.Wrappers;
using TiContent.Entities;
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
            
            // Подписываемся на события
            _mainWindow.Loaded += (_, _) => WeakReferenceMessenger.Default.Send(new WindowAction(WindowAction.ActionType.Loaded));
            _mainWindow.Closing += (_, _) => WeakReferenceMessenger.Default.Send(new WindowAction(WindowAction.ActionType.Unloaded));

            // Показываем окно
            _mainWindow.Show();
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _storageService.Save();
            return Task.CompletedTask;
        }

        public void Dispose()
        { }
    }
}