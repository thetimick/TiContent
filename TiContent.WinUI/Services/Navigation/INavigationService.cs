// ⠀
// INavigationService.cs
// TiContent.WinUI
// 
// Created by the_timick on 27.05.2025.
// ⠀

using Microsoft.UI.Xaml.Controls;

namespace TiContent.WinUI.Services.Navigation;

public interface INavigationService
{
    public void Setup(NavigationView view);
    public void NavigateTo(NavigationPath path);
    public void GoBack();
}