// ⠀
// ViewStateToVisibilityConverter.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 05.05.2025.
// ⠀

using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using TiContent.Foundation.Components.Abstractions;

namespace TiContent.UI.WinUI.Components.Converters;

public partial class ViewStateToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not ViewStateEnum viewState)
            return value;

        return parameter switch
        {
            "empty" => viewState switch
            {
                ViewStateEnum.Content => Visibility.Collapsed,
                ViewStateEnum.Empty => Visibility.Visible,
                ViewStateEnum.InProgress => Visibility.Collapsed,
                _ => throw new ArgumentOutOfRangeException(nameof(value)),
            },
            "progress" => viewState switch
            {
                ViewStateEnum.Content => Visibility.Collapsed,
                ViewStateEnum.Empty => Visibility.Collapsed,
                ViewStateEnum.InProgress => Visibility.Visible,
                _ => throw new ArgumentOutOfRangeException(nameof(value)),
            },
            "inv_content" => viewState switch
            {
                ViewStateEnum.Content => Visibility.Collapsed,
                ViewStateEnum.Empty => Visibility.Visible,
                ViewStateEnum.InProgress => Visibility.Visible,
                _ => throw new ArgumentOutOfRangeException(nameof(value)),
            },
            _ => viewState switch
            {
                ViewStateEnum.Content => Visibility.Visible,
                ViewStateEnum.Empty => Visibility.Collapsed,
                ViewStateEnum.InProgress => Visibility.Collapsed,
                _ => throw new ArgumentOutOfRangeException(nameof(value)),
            },
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }
}
