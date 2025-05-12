using TiContent.ViewModels.Pages;

namespace TiContent.Windows.Pages;

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