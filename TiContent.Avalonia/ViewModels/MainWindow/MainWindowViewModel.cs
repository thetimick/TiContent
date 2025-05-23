// ⠀
// MainWindowViewModel.cs
// TiContent.Avalonia
// 
// Created by the_timick on 18.05.2025.
// ⠀

using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentAvalonia.UI.Controls;
using TiContent.Avalonia.Windows.Main.Pages;

namespace TiContent.Avalonia.ViewModels.MainWindow;

public partial class MainWindowViewModel: ObservableObject
{
    // Observable
    
    [ObservableProperty]
    public partial ObservableCollection<object> MenuItems { get; set; } = [
        new NavigationViewItemHeader { Content = "Каталог" },
        new NavigationViewItem { IconSource = new SymbolIconSource { Symbol = Symbol.Library }, Content = "Фильмы & Сериалы", Tag = Items.Films },
        new NavigationViewItem { IconSource = new SymbolIconSource { Symbol = Symbol.Library }, Content = "Игры", Tag = Items.Games }
    ];
    
    [ObservableProperty]
    public partial ObservableCollection<object> FooterItems { get; set; } = [
        new NavigationViewItem { IconSource = new SymbolIconSource { Symbol = Symbol.Settings }, Content = "Настройки", Tag = Items.Settings },
        new NavigationViewItem { IconSource = new SymbolIconSource { Symbol = Symbol.Help }, Content = "О Программе", Tag = Items.About }
    ];
    
    [ObservableProperty]
    public partial NavigationViewItem? SelectedItem { get; set; }
    
    // Public Methods
    
    public void OnLoaded()
    {
        SelectedItem = MenuItems[1] as NavigationViewItem;
    }
}

public partial class MainWindowViewModel 
{
    public enum Items
    {
        Films,
        Games,
        Settings,
        About
    }
    
    public static Type GetItemType(Items item)
    {
        return item switch
        {
            Items.Films => typeof(FilmsPage),
            Items.Games => typeof(GamesPage),
            Items.Settings => typeof(SettingsPage),
            Items.About => typeof(AboutPage),
            _ => throw new ArgumentOutOfRangeException(nameof(item), item, null)
        };
    }
}