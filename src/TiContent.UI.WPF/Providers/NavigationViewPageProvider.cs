// ⠀
// NavigationViewPageProvider.cs
// TiContent.UI.WPF
//
// Created by the_timick on 30.03.2025.
// ⠀

using Wpf.Ui.Abstractions;

namespace TiContent.UI.WPF.Providers;

public class NavigationViewPageProvider(IServiceProvider provider) : INavigationViewPageProvider
{
    public object? GetPage(Type pageType)
    {
        return provider.GetService(pageType);
    }
}