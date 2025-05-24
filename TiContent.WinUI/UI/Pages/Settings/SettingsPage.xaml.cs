// ㅤ
// HomePage.xaml.cs
// TiContent.WinUI
// ㅤ
// Created by Timick on 16.12.2024.
// ㅤ

using Microsoft.UI.Xaml.Navigation;

namespace TiContent.WinUI.UI.Pages.Settings;

public partial class SettingsPage
{
    public SettingsPageViewModel ViewModel { get; set; } = null!;

    public SettingsPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        ViewModel = (SettingsPageViewModel)e.Parameter;
        base.OnNavigatedTo(e);
    }
}