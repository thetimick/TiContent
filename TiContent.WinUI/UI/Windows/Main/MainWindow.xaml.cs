using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Graphics;
using ABI.Microsoft.UI.Xaml.Media.Animation;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI.System;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using TiContent.WinUI.Services.Navigation;
using TiContent.WinUI.Services.Storage;
using WinUIEx;

namespace TiContent.WinUI.UI.Windows.Main;

public sealed partial class MainWindow
{
    // Public Props
    public MainWindowViewModel ViewModel { get; }

    // Private Props
    private readonly INavigationService _navigationService;

    // LifeCycle
    public MainWindow(IServiceProvider provider)
    {
        ViewModel = provider.GetRequiredService<MainWindowViewModel>();
        _navigationService = provider.GetRequiredService<INavigationService>();
        
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        
        ViewModel.OnLoaded();
        
        _navigationService.Setup(NavigationView);
        _navigationService.NavigateTo(NavigationPath.Films);
    }

    // Private Methods
    private void OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected)
            _navigationService.NavigateTo(NavigationPath.Settings);
        if (args.SelectedItem is NavigationViewItem { Tag: NavigationPath tag })
            _navigationService.NavigateTo(tag);
    }
}