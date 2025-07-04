// ⠀
// FilmsSourcesPage.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 26.05.2025.
// ⠀

using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace TiContent.UI.WinUI.UI.Pages.FilmsSource;

public sealed partial class FilmsSourcesPage
{
    private FilmsSourcesPageViewModel ViewModel { get; set; } = null!;

    public FilmsSourcesPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        ViewModel = (FilmsSourcesPageViewModel)e.Parameter;
        base.OnNavigatedTo(e);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        ViewModel.OnClosed();
        base.OnNavigatedFrom(e);
    }

    private void MenuFlyoutItem_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is SettingsCard { CommandParameter: string sParameter })
            ViewModel.TapOnTorrentButton(sParameter);

        if (sender is not MenuFlyoutItem { CommandParameter: string parameter, Tag: string tag })
            return;

        switch (tag)
        {
            case "0":
                ViewModel.TapOnTrackerButton(parameter);
            break;
            case "1":
                ViewModel.TapOnTorrentButton(parameter);
            break;
        }
    }
}