// ⠀
// PluralHelper.cs
// TiContent.UI.WPF.Foundation.Components
// 
// Created by the_timick on 31.05.2025.
// ⠀

namespace TiContent.Foundation.Components.Helpers;

public static class PluralHelper
{
    public static string Pluralize(int number, string form1, string form2, string form5)
    {
        number = Math.Abs(number);
        var n = number % 100;

        if (n is >= 11 and <= 14)
            return form5;

        return (n % 10) switch
        {
            1 => form1,
            2 or 3 or 4 => form2,
            _ => form5
        };
    }
}