using Avalonia.Interactivity;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Navigation;
using FluentAvalonia.UI.Windowing;
using MainWindowViewModel = TiContent.Avalonia.ViewModels.MainWindow.MainWindowViewModel;

namespace TiContent.Avalonia.Windows.Main;

public partial class MainWindow: AppWindow
{
    private MainWindowViewModel? ViewModel { get; }

    public MainWindow() { }
    public MainWindow(MainWindowViewModel viewModel, INavigationPageFactory pageFactory)
    {
        InitializeComponent();

        ViewModel = viewModel;
        DataContext = viewModel;
        
        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;

        SetupNavigation(pageFactory);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        ViewModel?.OnLoaded();
    }
}

public partial class MainWindow
{
    private void SetupNavigation(INavigationPageFactory navigationPageFactory)
    {
        NavigationViewFrame.NavigationPageFactory = navigationPageFactory;
        NavigationView.SelectionChanged += (_, args) =>
        {
            if (args.SelectedItemContainer.Tag is MainWindowViewModel.Items item)
                NavigationViewFrame.NavigateToType(MainWindowViewModel.GetItemType(item), item, new FrameNavigationOptions());
        };
    }
}