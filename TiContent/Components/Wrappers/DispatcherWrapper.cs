// ⠀
// DispatcherWrapper.cs
// TiContent
// 
// Created by the_timick on 28.04.2025.
// ⠀

namespace TiContent.Components.Wrappers;

public static class DispatcherWrapper
{
    public static void InvokeOnMain(Action action)
    {
        if (System.Windows.Application.Current.Dispatcher.CheckAccess())
            action.Invoke();
        System.Windows.Application.Current.Dispatcher.Invoke(action);

    }
    
    public static Task StartNewAsync(Action action, CancellationToken token = default)
    {
        return Task.Factory.StartNew(action, token);
    }
}