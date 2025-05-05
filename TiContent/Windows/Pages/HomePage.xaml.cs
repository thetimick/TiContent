using TiContent.ViewModels.Pages;
using Wpf.Ui;

namespace TiContent.Windows.Pages;

public partial class HomePage
{
    public HomePageViewModel ViewModel { get; }

    public HomePage(HomePageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = viewModel;

        InitializeComponent();
        
        Loaded += (_, _) => viewModel.LoadedCommand.Execute(null);
        Unloaded += (_, _) => viewModel.UnloadedCommand.Execute(null);
    }
}