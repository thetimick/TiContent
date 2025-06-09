// ⠀
// DependencyObjectHelper.cs
// TiContent.UI.WPF.UI.WinUI
// 
// Created by the_timick on 01.06.2025.
// ⠀

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace TiContent.UI.WinUI.Components.Helpers;

public static class DependencyObjectHelper
{
    public static T? FindVisualChild<T>(DependencyObject? parent) where T : DependencyObject
    {
        if (parent == null) 
            return null;

        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (VisualTreeHelper.GetChild(parent, i) is T result)
                return result;
            
            var foundChild = FindVisualChild<T>(child);
            if (foundChild != null) 
                return foundChild;
        }
        
        return null;
    }
}