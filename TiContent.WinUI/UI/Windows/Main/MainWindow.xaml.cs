using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System;
using TiContent.WinUI.UI.Pages.Games;
using TiContent.WinUI.UI.Pages.Settings;
using WinUIEx;
using FilmsPage = TiContent.WinUI.UI.Pages.Films.FilmsPage;
using FilmsPageViewModel = TiContent.WinUI.UI.Pages.Films.FilmsPageViewModel;

namespace TiContent.WinUI.UI.Windows.Main;

public sealed partial class MainWindow
{
    public MainWindowViewModel ViewModel { get; }

    private readonly IServiceProvider _provider;

    public MainWindow(MainWindowViewModel viewModel, IServiceProvider provider)
    {
        ViewModel = viewModel;
        _provider = provider;

        InitializeComponent();
        this.CenterOnScreen();

        ExtendsContentIntoTitleBar = true;
        ViewModel.LoadedCommand.Execute(null);
    }

    private void OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected)
            Navigate(MainWindowViewModel.Tag.Settings, sender);
        if (args.SelectedItem is NavigationViewItem { Tag: MainWindowViewModel.Tag tag })
            Navigate(tag, sender);
    }

    #region Helpers

    private void Navigate(MainWindowViewModel.Tag tag, NavigationView sender)
    {
        switch (tag)
        {
            case MainWindowViewModel.Tag.Films:
                sender.Header = MainWindowViewModel.Tag.Films.Humanize();
                var homePageViewModel = _provider.GetRequiredService<FilmsPageViewModel>();
                ContentFrame.Navigate(typeof(FilmsPage), homePageViewModel);
                break;
            
            case MainWindowViewModel.Tag.Games:
                sender.Header = MainWindowViewModel.Tag.Games.Humanize();
                var gamesPageViewModel = _provider.GetRequiredService<GamesPageViewModel>();
                ContentFrame.Navigate(typeof(GamesPage), gamesPageViewModel);
                break;

            case MainWindowViewModel.Tag.Settings:
                sender.Header = MainWindowViewModel.Tag.Settings.Humanize();
                var settingsPageViewModel = _provider.GetRequiredService<SettingsPageViewModel>();
                ContentFrame.Navigate(typeof(SettingsPage), settingsPageViewModel);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(tag), tag, null);
        }
    }

    #endregion
}