// ⠀
// NavigationPageProvider.cs
// TiContent.Avalonia
// 
// Created by the_timick on 19.05.2025.
// ⠀

using System;
using Avalonia.Controls;
using FluentAvalonia.UI.Controls;

namespace TiContent.Avalonia.Providers;

public class NavigationPageFactory(IServiceProvider provider): INavigationPageFactory
{
    public Control? GetPage(Type srcType)
    {
       return provider.GetService(srcType) as Control;
    }

    public Control? GetPageFromObject(object target)
    {
        return null;
    }
}