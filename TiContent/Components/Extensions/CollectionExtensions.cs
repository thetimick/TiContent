// ⠀
// CollectionExtensions.cs
// TiContent
// 
// Created by the_timick on 06.05.2025.
// ⠀

namespace TiContent.Components.Extensions;

public static class CollectionExtensions
{
    public static bool IsEmpty<T>(this IEnumerable<T>? source)
    {
        return source == null || !source.Any();
    }
}