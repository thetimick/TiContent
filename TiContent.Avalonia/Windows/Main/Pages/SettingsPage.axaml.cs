// ⠀
// SettingsPage.axaml.cs
// TiContent.Avalonia
// 
// Created by the_timick on 19.05.2025.
// ⠀

using Avalonia.Controls;
using TiContent.Avalonia.ViewModels.MainWindow.Pages;

namespace TiContent.Avalonia.Windows.Main.Pages;

public partial class SettingsPage : UserControl
{
    private SettingsPageViewModel? ViewModel { get; }

    public SettingsPage() { }
    public SettingsPage(SettingsPageViewModel viewModel)
    {
        InitializeComponent();
        
        ViewModel = viewModel;
        DataContext = ViewModel;
    }
}