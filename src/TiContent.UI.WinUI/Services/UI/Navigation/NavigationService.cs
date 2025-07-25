﻿// ⠀
// NavigationService.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 25.05.2025.
// ⠀

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Controls;
using TiContent.UI.WinUI.Providers;
using TiContent.UI.WinUI.UI.Pages.Films;
using TiContent.UI.WinUI.UI.Pages.FilmsSource;
using TiContent.UI.WinUI.UI.Pages.Games;
using TiContent.UI.WinUI.UI.Pages.GamesSource;
using TiContent.UI.WinUI.UI.Pages.GamesStatus;
using TiContent.UI.WinUI.UI.Pages.Settings;

namespace TiContent.UI.WinUI.Services.UI.Navigation;

public interface INavigationService
{
    public void Setup(NavigationView view);
    public void NavigateTo(NavigationPath path);
    public void GoBack();
}

public class NavigationService(IServiceProvider provider) : INavigationService
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
                _frame.Navigate(
                    typeof(FilmsPage),
                    new FilmsPage.Dependencies(
                        provider.GetRequiredService<FilmsPageViewModel>(),
                        provider.GetRequiredService<IImageProvider>(),
                        provider.GetRequiredService<ILogger<FilmsPage>>(),
                        provider.GetRequiredService<INotificationService>()
                    )
                );
            break;
            case NavigationPath.FilmsSource:
                _frame.Navigate(
                    typeof(FilmsSourcesPage),
                    provider.GetRequiredService<FilmsSourcesPageViewModel>()
                );
            break;

            case NavigationPath.Games:
                _frame.Navigate(
                    typeof(GamesPage),
                    new GamesPage.Dependencies(
                        provider.GetRequiredService<GamesPageViewModel>(),
                        provider.GetRequiredService<IImageProvider>(),
                        provider.GetRequiredService<ILogger<GamesPage>>(),
                        provider.GetRequiredService<INotificationService>()
                    )
                );
            break;

            case NavigationPath.GamesSource:
                _frame.Navigate(
                    typeof(GamesSourcePage),
                    provider.GetRequiredService<GamesSourcePageViewModel>()
                );
            break;

            case NavigationPath.Settings:
                _frame.Navigate(
                    typeof(SettingsPage),
                    provider.GetRequiredService<SettingsPageViewModel>()
                );
            break;

            case NavigationPath.GamesStatus:
                _frame.Navigate(
                    typeof(GamesStatusPage),
                    new GamesStatusPage.Dependencies(
                        provider.GetRequiredService<GamesStatusViewModel>(),
                        provider.GetRequiredService<IImageProvider>()
                    )
                );
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
        return type switch {
            not null when type == typeof(FilmsPage) => NavigationPath.Films,
            not null when type == typeof(FilmsSourcesPage) => NavigationPath.FilmsSource,
            not null when type == typeof(GamesPage) => NavigationPath.Games,
            not null when type == typeof(GamesSourcePage) => NavigationPath.GamesSource,
            not null when type == typeof(GamesStatusPage) => NavigationPath.GamesStatus,
            not null when type == typeof(SettingsPage) => NavigationPath.Settings,
            _ => null
        };
    }
}