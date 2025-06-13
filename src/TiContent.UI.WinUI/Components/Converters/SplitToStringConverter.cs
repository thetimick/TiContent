// ⠀
// SplitToStringConverter.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 13.06.2025.
// ⠀

using System;
using Microsoft.UI.Xaml.Data;
using TiContent.Foundation.Components.Extensions;

namespace TiContent.UI.WinUI.Components.Converters;

public partial class SplitToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not string stringValue || parameter is not string stringParameter)
            return value;

        var allParamsAsString = stringParameter.Split(",,,");
        var splitParam = allParamsAsString.GetSafe(0);
        var indexParam = int.Parse(allParamsAsString.GetSafe(1) ?? "-1");

        return stringValue.Split(splitParam).GetSafe(indexParam) ?? value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
