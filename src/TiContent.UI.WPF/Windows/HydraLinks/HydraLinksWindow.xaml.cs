using TiContent.UI.WPF.ViewModels.HydraLinks;

namespace TiContent.UI.WPF.Windows.HydraLinks;

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