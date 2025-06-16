// ⠀
// ImageDispatcherQueue.cs
// TiContent.UI.WinUI
// 
// Created by the_timick on 15.06.2025.
// ⠀

using Microsoft.UI.Dispatching;

namespace TiContent.UI.WinUI.Components.CustomDispatcherQueue;

public interface IImageDispatcherQueue
{
    public DispatcherQueue Queue { get; }
}

public class ImageDispatcherQueue : IImageDispatcherQueue
{
    public DispatcherQueue Queue { get; } = DispatcherQueueController.CreateOnDedicatedThread().DispatcherQueue;
}