// ㅤ
// App.ConfigureService.cs
// TiContent.UI.WinUI
// ㅤ
// Created by Timick on 16.12.2024.
// ㅤ

using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Graphics;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using TiContent.UI.WinUI.Services.DB;
using TiContent.UI.WinUI.Services.Storage;
using TiContent.UI.WinUI.Services.UI;
using TiContent.UI.WinUI.UI.Windows.Main;
using WinUIEx;

namespace TiContent.UI.WinUI;

public partial class App
{
    private partial class ConfigureService(
        IStorageService storage,
        IThemeService themeService,
        MainWindow window,
        IDataBaseGamesSourceService dbGamesSourceService,
        IDataBaseHydraFiltersService dbHydraFiltersService
    );

    // IHostedService

    private partial class ConfigureService : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            LoadStateFromStorage();
            SetupWindow();
            Task.Factory.StartNew(
                async () => await ObtainDataIfNeeded(cancellationToken),
                cancellationToken
            );
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            SaveStateToStorage();
            return Task.CompletedTask;
        }
    }

    // Private Methods

    private partial class ConfigureService
    {
        private void SetupWindow()
        {
            window.Activate();
            window.Closed += (_, _) => AppHost.StopAsync().Wait();
        }

        private async Task ObtainDataIfNeeded(CancellationToken token)
        {
            await Task.WhenAll(
                dbGamesSourceService.ObtainIfNeededAsync(false, token),
                dbHydraFiltersService.ObtainIfNeededAsync(token)
            );
        }

        private void LoadStateFromStorage()
        {
            storage.Obtain();

            themeService.ApplyTheme((ElementTheme)storage.Obtain().Window.ThemeIndex);
            window.AppWindow.Resize(
                storage.Cached.Window
                    is { IsWindowSizePersistent: true, Width: { } width, Height: { } height }
                    ? new SizeInt32(Convert.ToInt32(width), Convert.ToInt32(height))
                    : new SizeInt32(1280, 720)
            );
            if (storage.Cached.Window is { IsWindowOnCenterScreen: false, X: { } x, Y: { } y })
                window.AppWindow.Move(new PointInt32(Convert.ToInt32(x), Convert.ToInt32(y)));
            else
                window.CenterOnScreen();
        }

        private void SaveStateToStorage()
        {
            storage.Cached.Window.Width = window.AppWindow.Size.Width;
            storage.Cached.Window.Height = window.AppWindow.Size.Height;
            storage.Cached.Window.X = window.AppWindow.Position.X;
            storage.Cached.Window.Y = window.AppWindow.Position.Y;

            storage.Save();
        }
    }
}