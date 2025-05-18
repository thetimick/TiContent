// ⠀
// ImageCacheConverter.cs
// TiContent
// 
// Created by the_timick on 18.05.2025.
// ⠀

using System.Globalization;
using System.Windows.Data;
using TiContent.Application;
using TiContent.Providers;

namespace TiContent.Components.Converters;

public class StringToImageCacheConverter: IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // if (value is not string url) 
        //     return value;
        // var image = App.GetRequiredService<ImageCacheProvider>().GetImage(url);
        // return image.Data;

        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}