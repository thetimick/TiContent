// ㅤ
// HomePage.xaml.cs
// TiContent.WinUI
// ㅤ
// Created by Timick on 16.12.2024.
// ㅤ

using Microsoft.UI.Xaml.Navigation;

namespace TiContent.WinUI.UI.Pages.Films;

public partial class FilmsPage
{
    public FilmsPageViewModel ViewModel { get; private set; } = null!;

    public FilmsPage()
    {
        InitializeComponent();
        Loaded += (_, _) => ViewModel.LoadedCommand.Execute(null);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        ViewModel = (FilmsPageViewModel)e.Parameter;
        base.OnNavigatedTo(e);
    }
}