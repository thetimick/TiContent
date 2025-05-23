// ⠀
// AboutPage.axaml.cs
// TiContent.Avalonia
// 
// Created by the_timick on 19.05.2025.
// ⠀

using Avalonia.Controls;
using TiContent.Avalonia.ViewModels.MainWindow.Pages;

namespace TiContent.Avalonia.Windows.Main.Pages;

public partial class AboutPage : UserControl
{
    private AboutPageViewModel? ViewModel { get; }

    public AboutPage() { }
    public AboutPage(AboutPageViewModel viewModel)
    {
        InitializeComponent();
        
        ViewModel = viewModel;
        DataContext = ViewModel;
    }
}