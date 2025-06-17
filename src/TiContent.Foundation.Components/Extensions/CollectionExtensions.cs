// ⠀
// CollectionExtensions.cs
// TiContent.UI.WPF
//
// Created by the_timick on 06.05.2025.
// ⠀

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace TiContent.Foundation.Components.Extensions;

public static class CollectionExtensions
{
    public static bool IsEmpty<T>([NotNullWhen(false)] this IEnumerable<T>? source)
    {
        return source == null || !source.Any();
    }

    public static ObservableCollection<T> ToObservable<T>(this IEnumerable<T> source)
    {
        return new ObservableCollection<T>(source);
    }

    public static T? GetSafe<T>(this IEnumerable<T?> source, int index)
    {
        if (source is IList<T> list)
            return index >= 0 && index < list.Count ? list[index] : default;
        return index >= 0 ? source.Skip(index).Take(1).FirstOrDefault() : default;
    }
}