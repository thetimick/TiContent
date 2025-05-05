// ⠀
// DispatcherWrapper.cs
// TiContent
// 
// Created by the_timick on 28.04.2025.
// ⠀

using TiContent.Application;

namespace TiContent.Components.Wrappers;

public static class DispatcherWrapper
{
    public static void InvokeOnMain(Action action)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(action);
    }
}