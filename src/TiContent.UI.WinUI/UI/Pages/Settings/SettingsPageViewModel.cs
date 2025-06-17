// ⠀
// SettingsPageViewModel.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 24.05.2025.
// ⠀

using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using TiContent.Foundation.Constants;
using TiContent.UI.WinUI.Components.CustomDispatcherQueue;
using TiContent.UI.WinUI.Services.DB;
using TiContent.UI.WinUI.Services.Storage;
using TiContent.UI.WinUI.Services.UI;

namespace TiContent.UI.WinUI.UI.Pages.Settings;

public partial class SettingsPageViewModel : ObservableObject
{
    [ObservableProperty]
    public partial int ThemeIndex { get; set; }

    [ObservableProperty]
    public partial bool IsWindowSizePersistent { get; set; }

    [ObservableProperty]
    public partial bool IsWindowOnCenterScreen { get; set; }

    [ObservableProperty]
    public partial string TMDBApiKey { get; set; } = string.Empty;

    [ObservableProperty]
    public partial FilmsSettings Films { get; set; }

    [ObservableProperty]
    public partial GamesSourcesSettings GamesSources { get; set; }

    partial void OnThemeIndexChanged(int value)
    {
        if (_storageService.Cached.Window.ThemeIndex == value)
            return;
        _storageService.Cached.Window.ThemeIndex = value;
        _themeService.ApplyTheme((ElementTheme)value);
    }

    partial void OnIsWindowSizePersistentChanged(bool value)
    {
        if (_storageService.Cached.Window.IsWindowSizePersistent == value)
            return;
        _storageService.Cached.Window.IsWindowSizePersistent = value;
    }

    partial void OnIsWindowOnCenterScreenChanged(bool value)
    {
        if (_storageService.Cached.Window.IsWindowOnCenterScreen == value)
            return;
        _storageService.Cached.Window.IsWindowOnCenterScreen = value;
    }

    partial void OnTMDBApiKeyChanged(string value)
    {
        if (_storageService.Cached.Keys.TMDBApiKey == value)
            return;
        _storageService.Cached.Keys.TMDBApiKey = value;
    }

    private readonly IStorageService _storageService;
    private readonly IThemeService _themeService;

    public SettingsPageViewModel(
        IStorageService storageService,
        IThemeService themeService,
        IDataBaseGamesSourceService dbGamesSourceService
    )
    {
        _storageService = storageService;
        _themeService = themeService;

        Films = new FilmsSettings(storageService);
        GamesSources = new GamesSourcesSettings(storageService, dbGamesSourceService);
    }

    public void OnLoaded()
    {
        if (_storageService.Cached is not { } cached)
            return;

        ThemeIndex = cached.Window.ThemeIndex;
        IsWindowSizePersistent = cached.Window.IsWindowSizePersistent;
        IsWindowOnCenterScreen = cached.Window.IsWindowOnCenterScreen;
        TMDBApiKey = cached.Keys.TMDBApiKey;
    }

    [RelayCommand]
    private void TapOnOpenStorageButton()
    {
        Process.Start(
            new ProcessStartInfo(
                "explorer.exe",
                $"/select,\"{AppConstants.FileNames.DataBaseFileName}\""
            )
        );
    }
}

public partial class SettingsPageViewModel
{
    public partial class FilmsSettings(IStorageService storageService) : ObservableObject
    {
        [ObservableProperty]
        public partial int PosterQualityIndex { get; set; } = storageService.Cached.Films.PosterQualityIndex;

        partial void OnPosterQualityIndexChanged(int value)
        {
            storageService.Cached.Films.PosterQualityIndex = value;
        }
    }

    public partial class GamesSourcesSettings : ObservableObject,
                                                IRecipient<IDataBaseGamesSourceService.StateEventEntity>
    {
        [ObservableProperty]
        public partial bool UseOnlyTrustedSources { get; set; } = true;

        [ObservableProperty]
        public partial bool UseOnlyTrustedSourcesInProgress { get; set; } = false;

        partial void OnUseOnlyTrustedSourcesChanged(bool value)
        {
            if (value == _storageService.Cached.GamesSource.UseOnlyTrustedSources)
                return;

            _storageService.Cached.GamesSource.UseOnlyTrustedSources = value;
            Task.Factory.StartNew(() => _dbGamesSourceService.ObtainIfNeededAsync(true));
        }

        private readonly IStorageService _storageService;
        private readonly IDataBaseGamesSourceService _dbGamesSourceService;

        public GamesSourcesSettings(
            IStorageService storageService,
            IDataBaseGamesSourceService dbGamesSourceService
        )
        {
            _storageService = storageService;
            _dbGamesSourceService = dbGamesSourceService;

            UseOnlyTrustedSources = _storageService.Cached.GamesSource.UseOnlyTrustedSources;
            UseOnlyTrustedSourcesInProgress = dbGamesSourceService.InProgress;

            WeakReferenceMessenger.Default.Register(this);
        }

        public void Receive(IDataBaseGamesSourceService.StateEventEntity message)
        {
            App.GetRequiredService<IMainDispatcherQueue>().Queue
                .TryEnqueue(() => UseOnlyTrustedSourcesInProgress = message.InProgress);
        }
    }
}