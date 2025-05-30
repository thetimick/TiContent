﻿// ㅤ
// MainWindowViewModel.cs
// TiContent.WinUI
// ㅤ
// Created by Timick on 16.12.2024.
// ㅤ

using CommunityToolkit.Mvvm.ComponentModel;
using Humanizer;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using TiContent.WinUI.Services.Navigation;

namespace TiContent.WinUI.UI.Windows.Main;

public partial class MainWindowViewModel(INavigationService navService) : ObservableObject
{
    [ObservableProperty]
    public partial ObservableCollection<object> MenuItems { get; set; } = [
        new NavigationViewItem
        {
            Icon = new SymbolIcon(Symbol.Video),
            Content = NavigationPath.Films.Humanize(),
            Tag = NavigationPath.Films
        },
        new NavigationViewItem
        {
            Icon = new SymbolIcon(Symbol.Library),
            Content = NavigationPath.Games.Humanize(),
            Tag = NavigationPath.Games
        }
    ];
        
    [ObservableProperty]
    public partial object? NavigationViewSelectedItem { get; set; }

    // Public Methods
    
    public void OnLoaded()
    {
        NavigationViewSelectedItem = MenuItems[0];
        navService.NavigateTo(NavigationPath.Films);
    }
}