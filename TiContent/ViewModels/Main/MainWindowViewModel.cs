// ⠀
// MainWindowViewModel.cs
// TiContent
// 
// Created by the_timick on 30.03.2025.
// ⠀

using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using TiContent.Resources.Localization;
using TiContent.Windows.Main.Pages;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace TiContent.ViewModels.Main;

public partial class MainWindowViewModel(INavigationService navigation) : ObservableObject
{
    // Observable
    
    [ObservableProperty] 
    public partial ObservableCollection<object> MenuItems { get; set; } = [
        // new NavigationViewItem(Strings.HomePage_Title, SymbolRegular.Home24, typeof(HomePage)),
        new NavigationViewItemHeader
        {
            Text = Strings.MainWindow_NavigationItemHeader,
            FontSize = 14, 
            FontWeight = FontWeights.SemiBold,
            Margin = new Thickness(6,4,0,5)
        },
        new NavigationViewItem(Strings.FilmsPage_Title, SymbolRegular.Filmstrip24, typeof(FilmsPage)),
        new NavigationViewItem(Strings.GamesPage_Title, SymbolRegular.Games24, typeof(GamesPage))
    ];

    [ObservableProperty]
    public partial ObservableCollection<object> FooterItems { get; set; } = [
        new NavigationViewItem(Strings.SettingsPage_Title, SymbolRegular.Settings24, typeof(SettingsPage)),
        new NavigationViewItem(Strings.AboutPage_Title, SymbolRegular.Info24, typeof(AboutPage))
    ];
    
    // Public Methods

    public void OnLoaded()
    {
        navigation.Navigate(typeof(GamesPage));
    }
}