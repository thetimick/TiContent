// ⠀
// ByteSizeToStringConverter.cs
// TiContent
// 
// Created by the_timick on 28.04.2025.
// ⠀

using System;
using Humanizer.Bytes;
using Microsoft.UI.Xaml.Data;

namespace TiContent.WinUI.Components.Converters;

public class ByteSizeToStringConverter: IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is ByteSize size)
            return size.ToString();

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}