// ⠀
// GamesPage.axaml.cs
// TiContent.Avalonia
// 
// Created by the_timick on 19.05.2025.
// ⠀

using Avalonia.Controls;
using TiContent.Avalonia.ViewModels.MainWindow.Pages;

namespace TiContent.Avalonia.Windows.Main.Pages;

public partial class GamesPage: UserControl
{
    private GamesPageViewModel? ViewModel { get; }

    public GamesPage() { }
    public GamesPage(GamesPageViewModel viewModel)
    {
        InitializeComponent();
        
        ViewModel = viewModel;
        DataContext = ViewModel;
    }
}