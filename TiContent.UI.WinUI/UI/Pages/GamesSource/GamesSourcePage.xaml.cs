using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace TiContent.UI.WinUI.UI.Pages.GamesSource;

public sealed partial class GamesSourcePage
{
    // Private Methods
    
    private GamesSourcePageViewModel ViewModel { get; set; } = null!;
    
    // LifeCycle
    
    public GamesSourcePage()
    {
        InitializeComponent();
    }
    
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        ViewModel = (GamesSourcePageViewModel)e.Parameter;
        base.OnNavigatedTo(e);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        ViewModel.OnClosed();
        base.OnNavigatedFrom(e);
    }
    
    // Private Methods
    
    private void SettingsCard_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is SettingsCard { CommandParameter: string link })
            ViewModel.TapOnItem(link);
    }
}