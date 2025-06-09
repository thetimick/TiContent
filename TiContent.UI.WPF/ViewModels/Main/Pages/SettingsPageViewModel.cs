// ⠀
// SettingsPageViewModel.cs
// TiContent.UI.WPF
// 
// Created by the_timick on 30.03.2025.
// ⠀

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TiContent.UI.WPF.Components.Wrappers;
using TiContent.UI.WPF.Services.Storage;

namespace TiContent.UI.WPF.ViewModels.Main.Pages;

public partial class SettingsPageViewModel(
    IStorageService storageService
) : ObservableObject {

    [ObservableProperty]
    public partial int ThemeIndex { get; set; }

    [ObservableProperty]
    public partial bool IsWindowSizePersistent { get; set; }

    [ObservableProperty]
    public partial bool IsWindowOnCenterScreen { get; set; }

    partial void OnThemeIndexChanged(int value)
    {
        if (storageService.Cached != null)
            storageService.Cached.Window.ThemeIndex = value;
        ApplicationThemeManagerWrapper.Apply(value);
    }

    partial void OnIsWindowSizePersistentChanged(bool value)
    {
        if (storageService.Cached != null)
            storageService.Cached.Window.IsWindowSizePersistent = value;
    }

    partial void OnIsWindowOnCenterScreenChanged(bool value)
    {
        if (storageService.Cached != null)
            storageService.Cached.Window.IsWindowOnCenterScreen = !value;
    }

    [RelayCommand]
    private void OnLoaded()
    {
        ThemeIndex = storageService.Cached?.Window.ThemeIndex ?? 2;
        IsWindowSizePersistent = storageService.Cached?.Window.IsWindowSizePersistent ?? true;
        IsWindowOnCenterScreen = !(storageService.Cached?.Window.IsWindowOnCenterScreen ?? false);
    }
}