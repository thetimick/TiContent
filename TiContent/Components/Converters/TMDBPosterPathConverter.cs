// ⠀
// TMDBPosterPathConverter.cs
// TiContent
// 
// Created by the_timick on 12.05.2025.
// ⠀

using System.Globalization;
using System.Windows.Data;
using TiContent.Providers;

namespace TiContent.Components.Converters;

[ValueConversion(typeof(string), typeof(string))]
public class TMDBPosterPathConverter: IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string path)
            return TMDBPosterProvider.Provide(path);
        return value;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}