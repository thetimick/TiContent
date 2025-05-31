using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using Windows.Graphics;
using TiContent.WinUI.Services.Navigation;
using TiContent.WinUI.UI.Pages.Games;
using TiContent.WinUI.UI.Pages.Settings;
using WinUIEx;
using FilmsPage = TiContent.WinUI.UI.Pages.Films.FilmsPage;
using FilmsPageViewModel = TiContent.WinUI.UI.Pages.Films.FilmsPageViewModel;

namespace TiContent.WinUI.UI.Windows.Main;

public sealed partial class MainWindow
{
    public MainWindowViewModel ViewModel { get; }

    private readonly INavigationService _navigationService;

    public MainWindow(IServiceProvider provider)
    {
        ViewModel = provider.GetRequiredService<MainWindowViewModel>();
        _navigationService = provider.GetRequiredService<INavigationService>();
        
        InitializeComponent();
        
        AppWindow.Resize(new SizeInt32(1280, 720));
        this.CenterOnScreen();

        ExtendsContentIntoTitleBar = true;
        ViewModel.OnLoaded();
        
        _navigationService.Setup(NavigationView);
        _navigationService.NavigateTo(NavigationPath.Films);
    }

    private void OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected)
            _navigationService.NavigateTo(NavigationPath.Settings);
        if (args.SelectedItem is NavigationViewItem { Tag: NavigationPath tag })
            _navigationService.NavigateTo(tag);
    }
}