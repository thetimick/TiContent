// ⠀
// RegexHelper.cs
// TiContent.UI.WPF
// 
// Created by the_timick on 16.05.2025.
// ⠀

using System.Text.RegularExpressions;

namespace TiContent.Foundation.Components.Helpers;

public static partial class RegexHelper
{
    [GeneratedRegex("[^a-zA-Z0-9\u0400-\u04FF]")]
    public static partial Regex Clean();
}