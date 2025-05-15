using TiContent.ViewModels.HydraLinks;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace TiContent.Windows.HydraLinks;

public partial class HydraLinksWindow
{
    private HydraLinksWindowViewModel _viewModel;
    
    public HydraLinksWindow(HydraLinksWindowViewModel viewModel)
    {
        _viewModel = viewModel;
        DataContext = viewModel;
        
        InitializeComponent();
    }
}