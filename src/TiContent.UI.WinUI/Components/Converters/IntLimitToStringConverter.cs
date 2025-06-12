// ⠀
// IntLimitToStringConverter.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 07.06.2025.
// ⠀

using System;
using Microsoft.UI.Xaml.Data;

namespace TiContent.UI.WinUI.Components.Converters;

public partial class IntLimitToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not int intValue)
            return value;

        var max = 9999;
        if (parameter is string stringMax)
            max = int.Parse(stringMax);

        return intValue > max ? $"{max}+" : intValue.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
