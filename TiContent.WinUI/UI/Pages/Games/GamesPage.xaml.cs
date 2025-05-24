// ㅤ
// HomePage.xaml.cs
// TiContent.WinUI
// ㅤ
// Created by Timick on 16.12.2024.
// ㅤ

using Microsoft.UI.Xaml.Navigation;
using TiContent.WinUI.UI.Pages.Films;

namespace TiContent.WinUI.UI.Pages.Games;

public partial class GamesPage
{
    public GamesPageViewModel ViewModel { get; private set; } = null!;

    public GamesPage()
    {
        InitializeComponent();
        Loaded += (_, _) => ViewModel.LoadedCommand.Execute(null);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        ViewModel = (GamesPageViewModel)e.Parameter;
        base.OnNavigatedTo(e);
    }
}