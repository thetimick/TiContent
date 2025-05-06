// ⠀
// GlobalMessage.cs
// TiContent
// 
// Created by the_timick on 05.05.2025.
// ⠀

namespace TiContent.Components.Abstractions;

public class WindowAction(WindowAction.ActionType action)
{
    public enum ActionType
    {
        Loaded,
        Unloaded
    }
    
    public ActionType Action { get; set; } = action;
}