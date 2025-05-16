// ⠀
// ViewStateToVisibilityConverter.cs
// TiContent
// 
// Created by the_timick on 05.05.2025.
// ⠀

using System.Globalization;
using System.Windows;
using System.Windows.Data;
using TiContent.Components.Abstractions;

namespace TiContent.Components.Converters;

[ValueConversion(typeof(ViewStateEnum), typeof(Visibility))]
public class ViewStateToVisibilityConverter: IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not ViewStateEnum viewState)
            return value;

        return parameter switch
        {
            "empty" => viewState switch
            {
                ViewStateEnum.Content => Visibility.Hidden,
                ViewStateEnum.Empty => Visibility.Visible,
                ViewStateEnum.InProgress => Visibility.Hidden,
                _ => throw new ArgumentOutOfRangeException(nameof(value))
            },
            "progress" => viewState switch
            {
                ViewStateEnum.Content => Visibility.Hidden,
                ViewStateEnum.Empty => Visibility.Hidden,
                ViewStateEnum.InProgress => Visibility.Visible,
                _ => throw new ArgumentOutOfRangeException(nameof(value))
            },
            "inv_content" => viewState switch
            {
                ViewStateEnum.Content => Visibility.Hidden,
                ViewStateEnum.Empty => Visibility.Visible,
                ViewStateEnum.InProgress => Visibility.Visible,
                _ => throw new ArgumentOutOfRangeException(nameof(value))
            },
            _ => viewState switch
            {
                ViewStateEnum.Content => Visibility.Visible,
                ViewStateEnum.Empty => Visibility.Hidden,
                ViewStateEnum.InProgress => Visibility.Hidden,
                _ => throw new ArgumentOutOfRangeException(nameof(value))
            }
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}