// ⠀
// FilmsPage.xaml.cs
// TiContent
// 
// Created by the_timick on 06.05.2025.
// ⠀

using System.Windows;
using System.Windows.Controls;
using TiContent.ViewModels.Main.Pages;

namespace TiContent.Windows.Main.Pages;

public partial class FilmsPage
{
    private FilmsPageViewModel ViewModel { get; }
    
    public FilmsPage(FilmsPageViewModel viewModel)
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