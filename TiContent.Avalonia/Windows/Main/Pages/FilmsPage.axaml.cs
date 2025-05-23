// ⠀
// FilmsPage.axaml.cs
// TiContent.Avalonia
// 
// Created by the_timick on 19.05.2025.
// ⠀

using AsyncImageLoader;
using AsyncImageLoader.Loaders;
using Avalonia.Controls;
using Avalonia.Interactivity;
using TiContent.Avalonia.ViewModels.MainWindow.Pages;

namespace TiContent.Avalonia.Windows.Main.Pages;

public partial class FilmsPage : UserControl
{
    private FilmsPageViewModel? ViewModel { get; }

    public FilmsPage() { }
    public FilmsPage(FilmsPageViewModel viewModel)
    {
        InitializeComponent();
        
        ViewModel = viewModel;
        DataContext = ViewModel;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        ViewModel?.OnLoaded();
        base.OnLoaded(e);
    }
}