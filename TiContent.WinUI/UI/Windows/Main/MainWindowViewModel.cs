// ㅤ
// MainWindowViewModel.cs
// TiContent.WinUI
// ㅤ
// Created by Timick on 16.12.2024.
// ㅤ

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Humanizer;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TiContent.WinUI.UI.Windows.Main;

public partial class MainWindowViewModel : ObservableObject
{
    public enum Tag
    {
        [Description("Фильмы & Сериалы")]
        Films,
        [Description("Игры")]
        Games,
        [Description("Параметры")]
        Settings
    }

    [ObservableProperty]
    public partial ObservableCollection<object> MenuItems { get; set; } = [
        new NavigationViewItem
        {
            Icon = new SymbolIcon(Symbol.Video),
            Content = Tag.Films.Humanize(),
            Tag = Tag.Films
        },
        new NavigationViewItem
        {
            Icon = new SymbolIcon(Symbol.Library),
            Content = Tag.Games.Humanize(),
            Tag = Tag.Games
        }
    ];
        
    [ObservableProperty]
    public partial object? NavigationViewSelectedItem { get; set; }

    [RelayCommand]
    private void OnLoaded()
    {
        NavigationViewSelectedItem = MenuItems[0];
    }
}