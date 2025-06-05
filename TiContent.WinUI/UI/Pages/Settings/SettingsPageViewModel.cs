// ⠀
// SettingsPageViewModel.cs
// TiContent.WinUI
// 
// Created by the_timick on 24.05.2025.
// ⠀

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using TiContent.WinUI.Services.Storage;
using TiContent.WinUI.Services.Theme;

namespace TiContent.WinUI.UI.Pages.Settings;

public partial class SettingsPageViewModel(IStorageService storageService, IThemeService themeService) : ObservableObject
{
    [ObservableProperty]
    public partial int ThemeIndex { get; set; }
    
    [ObservableProperty]
    public partial bool IsWindowSizePersistent { get; set; }

    [ObservableProperty]
    public partial bool IsWindowOnCenterScreen { get; set; }
    
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

    public void OnLoaded()
    {
        if (storageService.Cached is not { } cached) 
            return;
        
        ThemeIndex = cached.Window.ThemeIndex;
        IsWindowSizePersistent = cached.Window.IsWindowSizePersistent;
        IsWindowOnCenterScreen = cached.Window.IsWindowOnCenterScreen;
    }
}