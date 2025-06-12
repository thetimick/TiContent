// ⠀
// ByteSizeToStringConverter.cs
// TiContent.UI.WPF
//
// Created by the_timick on 28.04.2025.
// ⠀

using System.Globalization;
using System.Windows.Data;
using Humanizer.Bytes;

namespace TiContent.UI.WPF.Components.Converters;

public class ByteSizeToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ByteSize size)
            return size.ToString();

        return value;
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        throw new NotImplementedException();
    }
}
