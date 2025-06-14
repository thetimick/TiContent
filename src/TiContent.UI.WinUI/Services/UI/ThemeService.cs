// ⠀
// ThemeService.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 06.06.2025.
// ⠀

using System;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using TiContent.UI.WinUI.UI.Windows.Main;

namespace TiContent.UI.WinUI.Services.UI;

public interface IThemeService
{
    public void ApplyTheme(ElementTheme theme);
    public ElementTheme GetCurrentTheme();
}

public partial class ThemeService(MainWindow window);

public partial class ThemeService : IThemeService
{
    public void ApplyTheme(ElementTheme theme)
    {
        ((FrameworkElement)window.Content).RequestedTheme = theme;
        window.AppWindow.TitleBar.PreferredTheme = theme switch {
            ElementTheme.Default => TitleBarTheme.UseDefaultAppMode,
            ElementTheme.Light   => TitleBarTheme.Light,
            ElementTheme.Dark    => TitleBarTheme.Dark,
            _                    => throw new ArgumentOutOfRangeException(nameof(theme))
        };
    }

    public ElementTheme GetCurrentTheme()
    {
        return ((FrameworkElement)window.Content).ActualTheme;
    }
}
