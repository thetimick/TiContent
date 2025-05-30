﻿// ⠀
// NavigationService.cs
// TiContent.WinUI
// 
// Created by the_timick on 25.05.2025.
// ⠀

using System;
using System.Collections.Generic;
using System.Linq;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using TiContent.WinUI.UI.Pages.Films;
using TiContent.WinUI.UI.Pages.Games;
using TiContent.WinUI.UI.Pages.Settings;
using FilmsSourcesPage = TiContent.WinUI.UI.Pages.FilmsSource.FilmsSourcesPage;
using FilmsSourcesPageViewModel = TiContent.WinUI.UI.Pages.FilmsSource.FilmsSourcesPageViewModel;

namespace TiContent.WinUI.Services.Navigation;

public class NavigationService(IServiceProvider provider): INavigationService
{
    // Private Props
    
    private Frame? _frame;
    private NavigationPath? _currentPath;
    
    // INavigationService
    
    public void Setup(NavigationView view)
    {
        _frame = view.Content as Frame;
    }
    
    public void NavigateTo(NavigationPath path)
    {
        if (_frame == null || _currentPath == path)
            return;
        
        switch (path)
        {
            case NavigationPath.Films:
                _frame.Navigate(typeof(FilmsPage), provider.GetRequiredService<FilmsPageViewModel>());
                break;
            case NavigationPath.FilmsSources:
                _frame.Navigate(typeof(FilmsSourcesPage), provider.GetRequiredService<FilmsSourcesPageViewModel>());
                break;
            
            case NavigationPath.Games:
                _frame.Navigate(typeof(GamesPage), provider.GetRequiredService<GamesPageViewModel>());
                break;
            case NavigationPath.GamesSources:
                break;
            
            case NavigationPath.Settings:
                _frame.Navigate(typeof(SettingsPage), provider.GetRequiredService<SettingsPageViewModel>());
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(path), path, null);
        }

        _currentPath = path;
    }

    public void GoBack()
    {
        _frame?.GoBack();
        _currentPath = GetCurrentPath(_frame?.CurrentSourcePageType);
    }

    // Helpers
    
    private static NavigationPath? GetCurrentPath(Type? type)
    {
        return type switch
        {
            not null when type == typeof(FilmsPage) => NavigationPath.Films,
            not null when type == typeof(FilmsSourcesPage) => NavigationPath.FilmsSources,
            not null when type == typeof(GamesPage) => NavigationPath.FilmsSources,
            not null when type == typeof(SettingsPage) => NavigationPath.Settings,
            _ => null
        };
    }
}