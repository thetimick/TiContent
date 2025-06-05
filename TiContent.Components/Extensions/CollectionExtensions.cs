// ⠀
// CollectionExtensions.cs
// TiContent
// 
// Created by the_timick on 06.05.2025.
// ⠀

using System.Collections.ObjectModel;

namespace TiContent.Components.Extensions;

public static class CollectionExtensions
{
    public static bool IsEmpty<T>(this IEnumerable<T>? source)
    {
        return source == null || !source.Any();
    }
    
    public static string ToStringPretty<T>(this IEnumerable<T> collection)
    {
        return "[" + string.Join(", ", collection.Select(item => item?.ToString() ?? "null")) + "]";
    }
}

public static class ObservableCollectionExtensions
{
    public static void AddRange<T>(this ObservableCollection<T> source, IEnumerable<T> items)
    {
        foreach (var item in items)
            source.Add(item);
    }
}