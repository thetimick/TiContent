using TiContent.ViewModels.Main.Pages;

namespace TiContent.Windows.Main.Pages;

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