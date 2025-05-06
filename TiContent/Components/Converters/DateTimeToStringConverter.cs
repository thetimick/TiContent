// ⠀
// DateTimeToStringConverter.cs
// TiContent
// 
// Created by the_timick on 05.05.2025.
// ⠀

using System.Globalization;
using System.Windows.Data;

namespace TiContent.Components.Converters;

public class DateTimeToStringConverter: IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime date && parameter is string format)
            return date.ToString(format, CultureInfo.InvariantCulture);
        
        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}