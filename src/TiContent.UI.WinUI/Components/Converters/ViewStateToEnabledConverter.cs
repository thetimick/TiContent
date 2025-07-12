// ⠀
// ViewStateToVisibilityConverter.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 05.05.2025.
// ⠀

using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using TiContent.Foundation.Abstractions.UI;

namespace TiContent.UI.WinUI.Components.Converters;

public partial class ViewStateToEnabledConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not ViewStateEnum viewState)
            return value;

        return viewState switch {
            ViewStateEnum.Content => true,
            _ => false
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }
}