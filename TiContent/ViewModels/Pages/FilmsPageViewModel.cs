// ⠀
// FilmsPageViewModel.cs
// TiContent
// 
// Created by the_timick on 06.05.2025.
// ⠀

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using TiContent.Components.Abstractions;

namespace TiContent.ViewModels.Pages;

public partial class FilmsPageViewModel: ObservableObject, IRecipient<WindowAction>
{
    // Lifecycle
    
    public FilmsPageViewModel()
    {
        WeakReferenceMessenger.Default.Register(this);
    }
    
    // Messenger
    
    public void Receive(WindowAction message)
    {
        switch (message.Action)
        {
            case WindowAction.ActionType.Loaded:
                break;
            case WindowAction.ActionType.Unloaded:
                break;
        }
    }
}