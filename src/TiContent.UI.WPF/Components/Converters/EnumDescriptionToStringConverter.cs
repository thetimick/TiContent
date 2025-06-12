// ⠀
// EnumDescriptionConverter.cs
// TiContent.UI.WPF
//
// Created by the_timick on 28.04.2025.
// ⠀

using System.Globalization;
using System.Windows.Data;
using Humanizer;

namespace TiContent.UI.WPF.Components.Converters;

public class EnumDescriptionToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || !value.GetType().IsEnum)
            return value?.ToString();

        return ((Enum)value).Humanize();
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        throw new NotSupportedException("Converting from description to enum is not supported.");
    }
}
