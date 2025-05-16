using TiContent.ViewModels.HydraLinks;

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