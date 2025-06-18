// ⠀
// VoteToColorConverter.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 25.05.2025.
// ⠀

using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace TiContent.UI.WinUI.Components.Converters;

public partial class VoteToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not string stringVote || !double.TryParse(stringVote, out var doubleVote) || doubleVote == 0)
            return Application.Current.Resources["TextFillColorSecondaryBrush"];

        return doubleVote switch {
            < 4 => Application.Current.Resources["SystemFillColorCriticalBrush"],
            < 7 => Application.Current.Resources["SystemFillColorCautionBrush"],
            _ => Application.Current.Resources["SystemFillColorSuccessBrush"]
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}