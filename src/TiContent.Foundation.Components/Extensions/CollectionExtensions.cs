// ⠀
// CollectionExtensions.cs
// TiContent.UI.WPF
// 
// Created by the_timick on 06.05.2025.
// ⠀

using System.Collections.ObjectModel;

namespace TiContent.Foundation.Components.Extensions;

public static class CollectionExtensions
{
    public static bool IsEmpty<T>(this IEnumerable<T>? source)
    {
        return source == null || !source.Any();
    }
    
    public static ObservableCollection<T> ToObservable<T>(this IEnumerable<T> source)
    {
        return new ObservableCollection<T>(source);
    }
}