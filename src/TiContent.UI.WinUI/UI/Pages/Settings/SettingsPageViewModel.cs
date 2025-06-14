// ⠀
// SettingsPageViewModel.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 24.05.2025.
// ⠀

using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using TiContent.Foundation.Constants;
using TiContent.UI.WinUI.Services.Storage;
using TiContent.UI.WinUI.Services.UI;

namespace TiContent.UI.WinUI.UI.Pages.Settings;

public partial class SettingsPageViewModel(IStorageService storageService, IThemeService themeService) : ObservableObject
{
    [ObservableProperty]
    public partial int ThemeIndex { get; set; }

    [ObservableProperty]
    public partial bool IsWindowSizePersistent { get; set; }

    [ObservableProperty]
    public partial bool IsWindowOnCenterScreen { get; set; }

    [ObservableProperty]
    public partial string TMDBApiKey { get; set; } = string.Empty;

    partial void OnThemeIndexChanged(int value)
    {
        if (storageService.Cached == null || storageService.Cached.Window.ThemeIndex == value)
            return;
        storageService.Cached.Window.ThemeIndex = value;
        themeService.ApplyTheme((ElementTheme)value);
    }

    partial void OnIsWindowSizePersistentChanged(bool value)
    {
        if (storageService.Cached == null || storageService.Cached.Window.IsWindowSizePersistent == value)
            return;
        storageService.Cached.Window.IsWindowSizePersistent = value;
    }

    partial void OnIsWindowOnCenterScreenChanged(bool value)
    {
        if (storageService.Cached == null || storageService.Cached.Window.IsWindowOnCenterScreen == value)
            return;
        storageService.Cached.Window.IsWindowOnCenterScreen = value;
    }

    partial void OnTMDBApiKeyChanged(string value)
    {
        if (storageService.Cached == null || storageService.Cached.Keys.TMDBApiKey == value)
            return;
        storageService.Cached.Keys.TMDBApiKey = value;
    }

    public void OnLoaded()
    {
        if (storageService.Cached is not { } cached)
            return;

        ThemeIndex = cached.Window.ThemeIndex;
        IsWindowSizePersistent = cached.Window.IsWindowSizePersistent;
        IsWindowOnCenterScreen = cached.Window.IsWindowOnCenterScreen;
        TMDBApiKey = cached.Keys.TMDBApiKey;
    }

    [RelayCommand]
    private void TapOnOpenStorageButton()
    {
        Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{AppConstants.FileNames.DataBaseFileName}\""));
    }
}
