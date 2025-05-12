// ⠀
// FilmsPage.xaml.cs
// TiContent
// 
// Created by the_timick on 06.05.2025.
// ⠀

using TiContent.ViewModels.Pages;

namespace TiContent.Windows.Pages;

public partial class FilmsPage
{
    public FilmsPageViewModel ViewModel { get; }
    
    public FilmsPage(FilmsPageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = viewModel;
        
        InitializeComponent();
    }
}