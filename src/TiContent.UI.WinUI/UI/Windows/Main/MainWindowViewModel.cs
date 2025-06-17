// ㅤ
// MainWindowViewModel.cs
// TiContent.UI.WinUI
// ㅤ
// Created by Timick on 16.12.2024.
// ㅤ

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Humanizer;
using Microsoft.UI.Xaml.Controls;
using TiContent.UI.WinUI.Services.UI.Navigation;

namespace TiContent.UI.WinUI.UI.Windows.Main;

public partial class MainWindowViewModel(INavigationService navService) : ObservableObject
{
    [ObservableProperty]
    public partial ObservableCollection<object> MenuItems { get; set; } = [
        new NavigationViewItemHeader { Content = "Каталог" },
        new NavigationViewItem {
            Icon = new FontIcon { Glyph = "\uE8B2" },
            Content = NavigationPath.Films.Humanize(),
            Tag = NavigationPath.Films
        },
        new NavigationViewItem {
            Icon = new FontIcon { Glyph = "\uE7FC" },
            Content = NavigationPath.Games.Humanize(),
            Tag = NavigationPath.Games
        }
    ];

    [ObservableProperty]
    public partial object? NavigationViewSelectedItem { get; set; }

    // Public Methods

    public void OnLoaded()
    {
        NavigationViewSelectedItem = MenuItems[1];
        navService.NavigateTo(NavigationPath.Films);
    }
}