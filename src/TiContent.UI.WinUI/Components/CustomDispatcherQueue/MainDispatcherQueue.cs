// ⠀
// MainDispatcherQueue.cs
// TiContent.UI.WinUI
// 
// Created by the_timick on 15.06.2025.
// ⠀

using Microsoft.UI.Dispatching;
using TiContent.UI.WinUI.UI.Windows.Main;

namespace TiContent.UI.WinUI.Components.CustomDispatcherQueue;

public interface IMainDispatcherQueue
{
    public DispatcherQueue Queue { get; }
}

public class MainDispatcherQueue(MainWindow window) : IMainDispatcherQueue
{
    public DispatcherQueue Queue { get; } = window.DispatcherQueue;
}