// ⠀
// DateTimeToStringConverter.cs
// TiContent.UI.WinUI
// 
// Created by the_timick on 05.05.2025.
// ⠀

using System;
using System.Globalization;
using Microsoft.UI.Xaml.Data;

namespace TiContent.UI.WinUI.Components.Converters;

public partial class DateTimeToStringConverter: IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is DateTime date && parameter is string format)
            return date.ToString(format, CultureInfo.InvariantCulture);
        
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}