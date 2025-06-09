using TiContent.UI.WPF.ViewModels.Main.Pages;

namespace TiContent.UI.WPF.Windows.Main.Pages;

public partial class AboutPage
{
    public AboutPageViewModel ViewModel { get; }

    public AboutPage(AboutPageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = viewModel;

        InitializeComponent();

        Loaded += (_, _) => viewModel.LoadedCommand.Execute(null);
    }
}