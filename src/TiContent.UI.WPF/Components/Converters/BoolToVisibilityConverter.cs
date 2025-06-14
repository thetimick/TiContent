// ⠀
// BoolToVisibilityConverter.cs
// TiContent.UI.WPF
//
// Created by the_timick on 05.05.2025.
// ⠀

using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TiContent.UI.WPF.Components.Converters;

public class BoolToVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool boolValue)
            return value;

        if (parameter is "inv")
            return boolValue ? Visibility.Hidden : Visibility.Visible;
        return boolValue ? Visibility.Visible : Visibility.Hidden;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
