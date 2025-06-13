// ⠀
// DoExtensions.cs
// TiContent.Foundation.Components
//
// Created by the_timick on 13.06.2025.
// ⠀

namespace TiContent.Foundation.Components.Extensions;

public static class DoExtensions
{
    public static T Do<T>(this T obj, Action<T> action)
    {
        action(obj);
        return obj;
    }

    public static async Task<T> DoAsync<T>(this T obj, Func<T, Task> action)
    {
        await action(obj);
        return obj;
    }
}
