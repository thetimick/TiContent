// ⠀
// NullToVisibilityConverter.cs
// TiContent.UI.WPF.UI.WinUI
// 
// Created by the_timick on 02.06.2025.
// ⠀

using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using TiContent.Foundation.Components.Extensions;

namespace TiContent.UI.WinUI.Components.Converters;

public partial class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, string language)
    {
        if (value is not string stringValue) 
            return value == null 
                ? Visibility.Collapsed
                : Visibility.Visible;
        
        return stringValue.IsNullOrEmpty() 
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}