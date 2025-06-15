// ⠀
// JacredWindow.xaml.cs
// TiContent.UI.WPF
//
// Created by the_timick on 16.05.2025.
// ⠀

using TiContent.UI.WPF.ViewModels.Jacred;

namespace TiContent.UI.WPF.Windows.Jacred;

public partial class JacredWindow
{
    private JacredWindowViewModel _viewModel;

    public JacredWindow(JacredWindowViewModel viewModel)
    {
        _viewModel = viewModel;
        DataContext = viewModel;

        InitializeComponent();
    }
}