using TiContent.ViewModels.Main.Pages;

namespace TiContent.Windows.Main.Pages;

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