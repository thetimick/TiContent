// ⠀
// MainWindowViewModel.cs
// TiContent
// 
// Created by the_timick on 30.03.2025.
// ⠀

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TiContent.Resources.Localization;
using TiContent.Windows.Pages;
using Wpf.Ui.Controls;

namespace TiContent.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    public partial ObservableCollection<object> MenuItems { get; set; } = [
        new NavigationViewItem(Strings.HomePage_Title, SymbolRegular.Home24, typeof(HomePage))
    ];

    [ObservableProperty]
    public partial ObservableCollection<object> FooterItems { get; set; } = [
        new NavigationViewItem(Strings.SettingsPage_Title, SymbolRegular.Settings24, typeof(SettingsPage)),
        new NavigationViewItem(Strings.AboutPage_Title, SymbolRegular.Info24, typeof(AboutPage))
    ];
}