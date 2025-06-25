// ㅤ
// GamesStatusPage.xaml.cs
// TiContent.UI.WinUI
// ㅤ
// Created by Timick on 16.12.2024.
// 

using Microsoft.UI.Xaml.Navigation;

namespace TiContent.UI.WinUI.UI.Pages.GamesStatus;

public sealed partial class GamesStatusPage
{
    public GamesStatusViewModel ViewModel { get; private set; } = null!;

    public GamesStatusPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is not Dependencies dependencies)
        {
            base.OnNavigatedTo(e);
            return;
        }

        ViewModel = dependencies.ViewModel;
    }
}

public sealed partial class GamesStatusPage
{
    public record Dependencies(
        GamesStatusViewModel ViewModel
    );
}