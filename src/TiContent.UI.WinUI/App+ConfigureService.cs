// ㅤ
// App.ConfigureService.cs
// TiContent.UI.WinUI
// ㅤ
// Created by Timick on 16.12.2024.
// ㅤ

using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.WinUI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using TiContent.UI.WinUI.Services.DB;
using TiContent.UI.WinUI.Services.Storage;
using TiContent.UI.WinUI.Services.UI;
using TiContent.UI.WinUI.UI.Windows.Main;
using Windows.Graphics;
using WinUIEx;

namespace TiContent.UI.WinUI;

public partial class App
{
    private class ConfigureService(IServiceProvider provider) : IHostedService
    {
        // Dependencies

        private readonly IStorageService _storage = provider.GetRequiredService<IStorageService>();
        private readonly IDataBaseGamesSourceService _dbGamesSourceService = provider.GetRequiredService<IDataBaseGamesSourceService>();
        private readonly IDataBaseHydraFiltersService _dbHydraFiltersService = provider.GetRequiredService<IDataBaseHydraFiltersService>();
        private readonly ILogger<ConfigureService> _logger = provider.GetRequiredService<ILogger<ConfigureService>>();
        private readonly IThemeService _themeService = provider.GetRequiredService<IThemeService>();
        private readonly MainWindow _window = provider.GetRequiredService<MainWindow>();
        private readonly AppDataBaseContext _db = provider.GetRequiredService<AppDataBaseContext>();
        private readonly INotificationService _notificationService = provider.GetRequiredService<INotificationService>();

        // IHostedService

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _storage.Obtain();
            ConfigureWindow();

            Task.Run(async () => await ConfigureDataBase(cancellationToken), cancellationToken);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _storage.Cached.Window.Width = _window.AppWindow.Size.Width;
            _storage.Cached.Window.Height = _window.AppWindow.Size.Height;
            _storage.Cached.Window.X = _window.AppWindow.Position.X;
            _storage.Cached.Window.Y = _window.AppWindow.Position.Y;

            _db.SaveChanges();
            _storage.Save();

            return Task.CompletedTask;
        }

        // Private Methods

        private void ConfigureWindow()
        {
            _themeService.ApplyTheme((ElementTheme)_storage.Obtain().Window.ThemeIndex);

            if (_storage.Cached is { } cached)
            {
                _window.AppWindow.Resize(
                    cached.Window is { IsWindowSizePersistent: true, Width: { } width, Height: { } height }
                        ? new SizeInt32(Convert.ToInt32(width), Convert.ToInt32(height))
                        : new SizeInt32(1280, 720)
                );

                if (cached.Window is { IsWindowOnCenterScreen: false, X: { } x, Y: { } y })
                    _window.AppWindow.Move(new PointInt32(Convert.ToInt32(x), Convert.ToInt32(y)));
                else
                    _window.CenterOnScreen();
            }

            _window.Activate();
            _window.Closed += async (_, _) => await AppHost.StopAsync();
        }

        private async Task ConfigureDataBase(CancellationToken token)
        {
            try
            {
                await _db.Database.MigrateAsync(token);
                await Task.WhenAll(_dbGamesSourceService.ObtainIfNeededAsync(token), _dbHydraFiltersService.ObtainIfNeededAsync(token));
            }
            catch (Exception ex)
            {
                await _window.DispatcherQueue.EnqueueAsync(() => _notificationService.ShowErrorNotification(ex));
                _logger.LogError(ex, "{ex}", ex.Message);
            }
        }
    }
}
