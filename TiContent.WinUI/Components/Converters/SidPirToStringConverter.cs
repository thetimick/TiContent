// ⠀
// SidPirToStringConverter.cs
// TiContent.WinUI
// 
// Created by the_timick on 31.05.2025.
// ⠀

using System;
using Microsoft.UI.Xaml.Data;

namespace TiContent.WinUI.Components.Converters;

public class SidPirToStringConverter: IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is not Tuple<int, int> tuple 
            ? value 
            : $"\u2191{tuple.Item1} \u2193{tuple.Item2}";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}