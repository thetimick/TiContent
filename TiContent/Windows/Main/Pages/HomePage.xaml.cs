using TiContent.ViewModels.Main.Pages;

namespace TiContent.Windows.Main.Pages;

public partial class HomePage
{
    public HomePageViewModel ViewModel { get; }

    public HomePage(HomePageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = viewModel;

        InitializeComponent();
    }
}