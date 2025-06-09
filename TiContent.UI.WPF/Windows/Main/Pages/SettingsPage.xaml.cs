using TiContent.UI.WPF.ViewModels.Main.Pages;

namespace TiContent.UI.WPF.Windows.Main.Pages;

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