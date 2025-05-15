using TiContent.Services.Storage;
using TiContent.Windows.Main.Pages;
using Wpf.Ui;
using MainWindowViewModel = TiContent.ViewModels.Main.MainWindowViewModel;

namespace TiContent.Windows.Main;

public partial class MainWindow
{
    private MainWindowViewModel ViewModel { get; }

    public MainWindow(
        MainWindowViewModel viewModel,
        INavigationService navigation,
        IStorageService storageService
    ) {
        ViewModel = viewModel;
        DataContext = viewModel;

        InitializeComponent();

        navigation.SetNavigationControl(NavigationView);

        SizeChanged += (_, args) =>
        {
            if (storageService.Cached is null)
                return;
            
            storageService.Cached.Window.Width = args.NewSize.Width;
            storageService.Cached.Window.Height = args.NewSize.Height;
        };

        LocationChanged += (_, _) =>
        {
            if (storageService.Cached is null)
                return;

            storageService.Cached.Window.X = Left;
            storageService.Cached.Window.Y = Top;
        };
        
        Loaded += (_, _) => ViewModel.OnLoaded();
    }
}