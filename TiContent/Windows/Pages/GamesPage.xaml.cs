// ⠀
// GamesPage.xaml.cs
// TiContent
// 
// Created by the_timick on 06.05.2025.
// ⠀

using System.Windows.Controls;
using TiContent.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace TiContent.Windows.Pages;

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