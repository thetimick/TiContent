using TiContent.Services.Storage;
using TiContent.ViewModels;
using TiContent.Windows.Pages;
using Wpf.Ui;

namespace TiContent.Windows;

public partial class MainWindow
{
    public MainWindowViewModel ViewModel { get; }

    public MainWindow(
        MainWindowViewModel viewModel,
        INavigationService navigationService,
        IStorageService storageService
    ) {
        ViewModel = viewModel;
        DataContext = viewModel;

        InitializeComponent();

        navigationService.SetNavigationControl(NavigationView);

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
        
        Loaded += (_, _) =>
        {
            NavigationView.Navigate(typeof(FilmsPage));
        };
    }
}