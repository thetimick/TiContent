// ⠀
// RegexHelper.cs
// TiContent
// 
// Created by the_timick on 16.05.2025.
// ⠀

using System.Text.RegularExpressions;

namespace TiContent.Components.Helpers;

public static partial class RegexHelper
{
    [GeneratedRegex("[^a-zA-Z0-9]")]
    public static partial Regex Clean();
}