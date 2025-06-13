// ㅤ
// App.ConfigureService.cs
// TiContent.UI.WinUI
// ㅤ
// Created by Timick on 16.12.2024.
// ㅤ

using System;
using System.Threading;
using System.Threading.Tasks;
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

        private readonly IStorageService _storageService =
            provider.GetRequiredService<IStorageService>();
        private readonly IDataBaseGamesSourceService _dbGamesSourceService =
            provider.GetRequiredService<IDataBaseGamesSourceService>();
        private readonly IDataBaseFiltersService _dbFiltersService =
            provider.GetRequiredService<IDataBaseFiltersService>();
        private readonly ILogger<ConfigureService> _logger = provider.GetRequiredService<
            ILogger<ConfigureService>
        >();
        private readonly IThemeService _themeService = provider.GetRequiredService<IThemeService>();
        private readonly MainWindow _window = provider.GetRequiredService<MainWindow>();
        private readonly AppDataBaseContext _db = provider.GetRequiredService<AppDataBaseContext>();

        // IHostedService

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _storageService.Obtain();
            ConfigureWindow();

            Task.Run(
                async () =>
                {
                    try
                    {
                        await _db.Database.MigrateAsync(cancellationToken);
                        await _dbGamesSourceService.ObtainItemsIfNeededAsync();
                        await _dbFiltersService.ObtainIfNeededAsync(cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "{ex}", ex.Message);
                    }
                },
                cancellationToken
            );

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_storageService.Cached != null)
            {
                _storageService.Cached.Window.Width = _window.AppWindow.Size.Width;
                _storageService.Cached.Window.Height = _window.AppWindow.Size.Height;
                _storageService.Cached.Window.X = _window.AppWindow.Position.X;
                _storageService.Cached.Window.Y = _window.AppWindow.Position.Y;
            }

            _db.SaveChanges();
            _storageService.Save();

            return Task.CompletedTask;
        }

        // Private Methods

        private void ConfigureWindow()
        {
            _themeService.ApplyTheme((ElementTheme)_storageService.Obtain().Window.ThemeIndex);

            if (_storageService.Cached is { } cached)
            {
                _window.AppWindow.Resize(
                    cached.Window
                        is { IsWindowSizePersistent: true, Width: { } width, Height: { } height }
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
    }
}
