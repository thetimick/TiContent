using TiContent.ViewModels.Pages;

namespace TiContent.Windows.Pages;

public partial class SettingsPage
{
    public SettingsPageViewModel ViewModel { get; }

    public SettingsPage(SettingsPageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = viewModel;

        InitializeComponent();

        Loaded += (_, _) => viewModel.LoadedCommand.Execute(null);
    }
}