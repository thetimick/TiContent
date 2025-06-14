// ⠀
// PluralHelper.cs
// TiContent.UI.WPF.Foundation.Components
//
// Created by the_timick on 31.05.2025.
// ⠀

namespace TiContent.Foundation.Components.Helpers;

public static class PluralHelper
{
    /// <summary>
    ///     Возвращает правильную форму слова в зависимости от заданного числа,
    ///     согласно правилам русской грамматики для склонения существительных по числам.
    /// </summary>
    /// <param name="number">Число, от которого зависит форма слова.</param>
    /// <param name="form1">Форма слова, используемая с числом, оканчивающимся на 1 (кроме 11), например: "яблоко".</param>
    /// <param name="form2">
    ///     Форма слова, используемая с числами, оканчивающимися на 2, 3 или 4 (кроме 12–14), например:
    ///     "яблока".
    /// </param>
    /// <param name="form5">Форма слова, используемая с остальными числами, например: "яблок".</param>
    /// <param name="withValue">
    ///     Если <c>true</c>, возвращает строку, содержащую число и форму слова (например, "5 яблок").
    ///     Если <c>false</c>, возвращает только форму слова без числа.
    /// </param>
    /// <returns>
    ///     Форма слова (с числом или без, в зависимости от <paramref name="withValue" />), соответствующая переданному
    ///     числу.
    /// </returns>
    public static string Pluralize(int number, string form1, string form2, string form5, bool withValue = false)
    {
        var absNumber = Math.Abs(number);
        var n = absNumber % 100;

        var word = n is >= 11 and <= 14
            ? form5
            : (n % 10) switch {
                1           => form1,
                2 or 3 or 4 => form2,
                _           => form5
            };

        return withValue ? $"{number} {word}" : word;
    }
}