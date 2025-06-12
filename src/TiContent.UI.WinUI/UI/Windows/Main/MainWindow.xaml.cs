using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using TiContent.UI.WinUI.Services.UI;
using TiContent.UI.WinUI.Services.UI.Navigation;

namespace TiContent.UI.WinUI.UI.Windows.Main;

public sealed partial class MainWindow
{
    // Public Props
    public MainWindowViewModel ViewModel { get; }

    // Private Props
    private readonly INavigationService _navigationService;
    private readonly INotificationService _notificationService;

    // LifeCycle
    public MainWindow(IServiceProvider provider)
    {
        ViewModel = provider.GetRequiredService<MainWindowViewModel>();

        _navigationService = provider.GetRequiredService<INavigationService>();
        _notificationService = provider.GetRequiredService<INotificationService>();

        InitializeComponent();
        ExtendsContentIntoTitleBar = true;

        ViewModel.OnLoaded();

        _navigationService.Setup(NavigationView);
        _navigationService.NavigateTo(NavigationPath.Films);

        _notificationService.Setup(InfoBarPanel);
    }

    // Private Methods
    private void OnSelectionChanged(
        NavigationView sender,
        NavigationViewSelectionChangedEventArgs args
    )
    {
        if (args.IsSettingsSelected)
            _navigationService.NavigateTo(NavigationPath.Settings);
        if (args.SelectedItem is NavigationViewItem { Tag: NavigationPath tag })
            _navigationService.NavigateTo(tag);
    }
}
