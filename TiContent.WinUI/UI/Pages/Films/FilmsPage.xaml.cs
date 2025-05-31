// ㅤ
// HomePage.xaml.cs
// TiContent.WinUI
// ㅤ
// Created by Timick on 16.12.2024.
// ㅤ

using System;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using TiContent.Components.Helpers;

namespace TiContent.WinUI.UI.Pages.Films;

public partial class FilmsPage
{
    public FilmsPageViewModel ViewModel { get; private set; } = null!;

    public FilmsPage()
    {
        InitializeComponent();
        Loaded += (_, _) => ViewModel.OnLoaded();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        ViewModel = (FilmsPageViewModel)e.Parameter;
        base.OnNavigatedTo(e);
    }

    private void ScrollViewer_OnViewChanged(object? sender, ScrollViewerViewChangedEventArgs e)
    {
       if (sender is ScrollViewer viewer)
           ViewModel.OnScrollChanged(viewer.VerticalOffset, viewer.ScrollableHeight);
    }

    private void MenuFlyoutItem_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem item)
            ViewModel.TapOnOpenButton(((string)item.CommandParameter, Enum.Parse<OpenHelper.Type>((string)item.Tag)));
    }

    private void JacredButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
            ViewModel.TapOnOpenFilmsSourceButton((string)button.CommandParameter);
    }
}