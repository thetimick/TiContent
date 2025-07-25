﻿// ⠀
// InvertBoolToVisibilityConverter.cs
// TiContent.UI.WinUI
// 
// Created by the_timick on 15.06.2025.
// ⠀

using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace TiContent.UI.WinUI.Components.Converters;

public partial class InvertBoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool boolValue)
            return boolValue ? Visibility.Collapsed : Visibility.Visible;
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}