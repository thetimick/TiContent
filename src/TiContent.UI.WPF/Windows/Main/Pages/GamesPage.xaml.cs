// ⠀
// GamesPage.xaml.cs
// TiContent.UI.WPF
// 
// Created by the_timick on 06.05.2025.
// ⠀

using System.Windows.Controls;
using TiContent.UI.WPF.ViewModels.Main.Pages;

namespace TiContent.UI.WPF.Windows.Main.Pages;

public partial class GamesPage
{
    public GamesPageViewModel ViewModel { get; }
    
    public GamesPage(GamesPageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = viewModel;
        
        InitializeComponent();

        Loaded += (_, _) => viewModel.OnLoaded();
    }

    private void ScrollViewer_OnScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        ViewModel.OnScrollChanged(e.VerticalOffset, (sender as ScrollViewer)?.ScrollableHeight ?? 0);
    }
}