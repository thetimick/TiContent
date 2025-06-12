// ⠀
// ApplicationThemeManagerWrapper.cs
// TiContent.UI.WPF
//
// Created by the_timick on 26.04.2025.
// ⠀

using Wpf.Ui.Appearance;

namespace TiContent.UI.WPF.Components.Wrappers;

public static class ApplicationThemeManagerWrapper
{
    public static void Apply(int index)
    {
        switch (index)
        {
            case 0:
                ApplicationThemeManager.Apply(ApplicationTheme.Light);
                break;
            case 1:
                ApplicationThemeManager.Apply(ApplicationTheme.Dark);
                break;
            case 2:
                ApplicationThemeManager.ApplySystemTheme(true);
                break;
        }
    }
}
