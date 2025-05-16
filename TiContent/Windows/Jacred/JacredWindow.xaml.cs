// ⠀
// JacredWindow.xaml.cs
// TiContent
// 
// Created by the_timick on 16.05.2025.
// ⠀

using TiContent.ViewModels.Jacred;

namespace TiContent.Windows.Jacred;

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